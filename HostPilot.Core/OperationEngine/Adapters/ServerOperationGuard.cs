using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class ServerOperationGuard(IOperationStateStore stateStore) : IOperationGuard
{
    public Task<OperationResult?> EvaluateAsync(OperationRequest request, CancellationToken cancellationToken)
    {
        if (!stateStore.TryGetServerState(request.TargetId, out var state) || state is null)
            return Task.FromResult<OperationResult?>(null);

        if (request.Type == OperationType.InstallServer && state.IsInstalled)
        {
            return Task.FromResult<OperationResult?>(OperationResult.Failure(
                $"Install skipped because '{request.TargetId}' is already installed.",
                retryable: false));
        }

        if (request.Type == OperationType.StartServer && state.IsRunning)
        {
            return Task.FromResult<OperationResult?>(OperationResult.Failure(
                $"Start skipped because '{request.TargetId}' is already running.",
                retryable: false));
        }

        if (request.Type == OperationType.StopServer && !state.IsRunning)
        {
            return Task.FromResult<OperationResult?>(OperationResult.Failure(
                $"Stop skipped because '{request.TargetId}' is not running.",
                retryable: false));
        }

        return Task.FromResult<OperationResult?>(null);
    }
}
