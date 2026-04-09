namespace HostPilot.Remote.Execution.Abstractions;

using HostPilot.Remote.Contracts.Models;

public interface IRemoteCommandExecutor
{
    Task<RemoteExecutionResult> ExecuteAsync(RemoteExecutionRequest request, RemoteCommandDescriptor descriptor, CancellationToken cancellationToken = default);
}
