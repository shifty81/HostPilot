namespace HostPilot.Remote.Execution.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Execution.Abstractions;

public sealed class RemoteCommandExecutionService
{
    private readonly RemoteCommandCatalog _catalog;
    private readonly ICommandExecutionPolicy _policy;
    private readonly IRemoteCommandExecutor _executor;
    private readonly ICommandExecutionSink _sink;

    public RemoteCommandExecutionService(
        RemoteCommandCatalog catalog,
        ICommandExecutionPolicy policy,
        IRemoteCommandExecutor executor,
        ICommandExecutionSink sink)
    {
        _catalog = catalog;
        _policy = policy;
        _executor = executor;
        _sink = sink;
    }

    public async Task<RemoteExecutionResult> ExecuteAsync(RemoteExecutionRequest request, CancellationToken cancellationToken = default)
    {
        if (!_catalog.TryGet(request.CommandKey, out var descriptor))
        {
            throw new KeyNotFoundException($"Unknown command '{request.CommandKey}'.");
        }

        request.CorrelationId ??= Guid.NewGuid().ToString("N");
        await _policy.EnsureAllowedAsync(request, descriptor, cancellationToken);

        await _sink.PublishProgressAsync(new RemoteExecutionProgress
        {
            CorrelationId = request.CorrelationId,
            NodeId = request.NodeId,
            CommandKey = request.CommandKey,
            Stage = "Dispatch",
            Message = $"Dispatching {descriptor.DisplayName}",
            Percent = 5,
        }, cancellationToken);

        var result = await _executor.ExecuteAsync(request, descriptor, cancellationToken);
        await _sink.PublishCompletedAsync(result, cancellationToken);
        return result;
    }
}
