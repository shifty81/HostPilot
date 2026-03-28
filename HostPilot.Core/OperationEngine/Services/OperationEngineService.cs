using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Configuration;
using HostPilot.Core.OperationEngine.Events;
using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Services;

public sealed class OperationEngineService : IOperationEngine, IDisposable
{
    private readonly OperationEngineOptions _options;
    private readonly IOperationStateStore _stateStore;
    private readonly IEventBus _eventBus;
    private readonly IReadOnlyList<IOperationGuard> _guards;
    private readonly OperationHandlerRegistry _handlerRegistry;
    private readonly OperationScheduler _scheduler = new();
    private readonly SemaphoreSlim _workerGate;
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly Task _pumpTask;

    public IEventBus EventBus => _eventBus;

    public OperationEngineService(
        OperationEngineOptions options,
        IOperationStateStore stateStore,
        IEventBus eventBus,
        IEnumerable<IOperationGuard> guards,
        IEnumerable<IOperationHandler> handlers)
    {
        _options = options;
        _stateStore = stateStore;
        _eventBus = eventBus;
        _guards = guards.ToArray();
        _handlerRegistry = new OperationHandlerRegistry(handlers);
        _workerGate = new SemaphoreSlim(Math.Max(1, options.MaxConcurrentOperations));
        _pumpTask = Task.Run(() => PumpAsync(_disposeCts.Token));
    }

    public Task<string> QueueAsync(OperationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Type);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TargetId);

        var record = new OperationRecord
        {
            OperationId = request.OperationId,
            Type = request.Type,
            TargetId = request.TargetId,
            Priority = request.Priority,
            MaxRetries = request.MaxRetries,
            Payload = request.Payload,
            DependsOnOperationIds = request.DependsOnOperationIds,
            Metadata = request.Metadata,
        };

        record.MarkQueued();
        record.AddLog("INFO", $"Queued {request.Type} for target '{request.TargetId}'.");
        _stateStore.UpsertOperation(record);
        _scheduler.Enqueue(request);
        _eventBus.Publish(new OperationQueuedEvent(record.OperationId, record.Type, record.TargetId));
        _eventBus.Publish(new OperationStatusChangedEvent(record.OperationId, record.Type, record.TargetId, record.Status));
        return Task.FromResult(record.OperationId);
    }

    public bool TryGetOperation(string operationId, out OperationRecord? record) => _stateStore.TryGetOperation(operationId, out record);

    public IReadOnlyCollection<OperationRecord> GetOperations() => _stateStore.GetAllOperations();

    public void RegisterHandler(IOperationHandler handler) => _handlerRegistry.Register(handler);

    private async Task PumpAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!_scheduler.TryAcquireNext(out var request) || request is null)
                {
                    await Task.Delay(_options.PollingDelayMilliseconds, cancellationToken);
                    continue;
                }

                if (!_stateStore.AreDependenciesSatisfied(request))
                {
                    _scheduler.Enqueue(request);
                    _scheduler.ReleaseTarget(request.TargetId);
                    await Task.Delay(_options.PollingDelayMilliseconds, cancellationToken);
                    continue;
                }

                await _workerGate.WaitAsync(cancellationToken);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await ExecuteRequestAsync(request, cancellationToken);
                    }
                    finally
                    {
                        _scheduler.ReleaseTarget(request.TargetId);
                        _workerGate.Release();
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task ExecuteRequestAsync(OperationRequest request, CancellationToken cancellationToken)
    {
        if (!_stateStore.TryGetOperation(request.OperationId, out var record) || record is null)
            return;

        foreach (var guard in _guards)
        {
            var guardResult = await guard.EvaluateAsync(request, cancellationToken);
            if (guardResult is null)
                continue;

            record.AddLog("WARN", guardResult.Message ?? "Operation rejected by guard.");
            record.MarkCompleted(OperationStatus.Skipped, guardResult);
            _stateStore.UpsertOperation(record);
            _eventBus.Publish(new OperationStatusChangedEvent(record.OperationId, record.Type, record.TargetId, record.Status));
            return;
        }

        if (!_handlerRegistry.TryResolve(request.Type, out var handler) || handler is null)
        {
            var notRegistered = OperationResult.Failure($"No operation handler registered for type '{request.Type}'.");
            record.AddLog("ERROR", notRegistered.Message!);
            record.MarkCompleted(OperationStatus.Failed, notRegistered);
            _stateStore.UpsertOperation(record);
            _eventBus.Publish(new OperationStatusChangedEvent(record.OperationId, record.Type, record.TargetId, record.Status));
            return;
        }

        using var timeoutCts = request.Timeout.HasValue
            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
            : null;
        if (timeoutCts is not null)
            timeoutCts.CancelAfter(request.Timeout.Value);

        var effectiveToken = timeoutCts?.Token ?? cancellationToken;
        var context = new OperationContext(record.OperationId, record.Type, record.TargetId, record.Metadata);

        while (true)
        {
            record.MarkRunning();
            record.AddLog("INFO", $"Starting attempt {record.AttemptCount}.");
            _stateStore.UpsertOperation(record);
            _eventBus.Publish(new OperationStatusChangedEvent(record.OperationId, record.Type, record.TargetId, record.Status));

            OperationResult result;
            try
            {
                result = await handler.ExecuteAsync(context, request.Payload, effectiveToken);
            }
            catch (OperationCanceledException ex)
            {
                result = OperationResult.Failure($"Operation '{record.Type}' was cancelled or timed out.", ex, retryable: false);
            }
            catch (Exception ex)
            {
                result = OperationResult.Failure($"Unhandled exception while executing '{record.Type}'.", ex, retryable: false);
            }

            record.AddLog(result.SuccessValue ? "INFO" : "ERROR", result.Message ?? (result.SuccessValue ? "Operation succeeded." : "Operation failed."));

            if (!result.SuccessValue && result.Retryable && record.AttemptCount <= record.MaxRetries)
            {
                record.MarkRetrying(result);
                _stateStore.UpsertOperation(record);
                _eventBus.Publish(new OperationStatusChangedEvent(record.OperationId, record.Type, record.TargetId, record.Status));
                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
                continue;
            }

            record.MarkCompleted(result.SuccessValue ? OperationStatus.Succeeded : OperationStatus.Failed, result);
            _stateStore.UpsertOperation(record);
            _eventBus.Publish(new OperationStatusChangedEvent(record.OperationId, record.Type, record.TargetId, record.Status));
            return;
        }
    }

    public void Dispose()
    {
        _disposeCts.Cancel();
        try { _pumpTask.GetAwaiter().GetResult(); } catch { }
        _workerGate.Dispose();
        _disposeCts.Dispose();
    }
}
