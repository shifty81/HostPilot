namespace HostPilot.Remote.Execution.Services;

using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Execution.Abstractions;

public sealed class DevelopmentRemoteCommandExecutor : IRemoteCommandExecutor
{
    public async Task<RemoteExecutionResult> ExecuteAsync(RemoteExecutionRequest request, RemoteCommandDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        return new RemoteExecutionResult
        {
            CorrelationId = request.CorrelationId ?? Guid.NewGuid().ToString("N"),
            NodeId = request.NodeId,
            CommandKey = request.CommandKey,
            Succeeded = true,
            Summary = $"Executed {descriptor.DisplayName} using development executor.",
        };
    }
}
