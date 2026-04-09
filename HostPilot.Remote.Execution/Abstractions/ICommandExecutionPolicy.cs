namespace HostPilot.Remote.Execution.Abstractions;

using HostPilot.Remote.Contracts.Models;

public interface ICommandExecutionPolicy
{
    Task EnsureAllowedAsync(RemoteExecutionRequest request, RemoteCommandDescriptor descriptor, CancellationToken cancellationToken = default);
}
