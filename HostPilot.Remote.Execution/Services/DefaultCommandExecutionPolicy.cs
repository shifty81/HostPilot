namespace HostPilot.Remote.Execution.Services;

using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Execution.Abstractions;

public sealed class DefaultCommandExecutionPolicy : ICommandExecutionPolicy
{
    public Task EnsureAllowedAsync(RemoteExecutionRequest request, RemoteCommandDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NodeId))
        {
            throw new InvalidOperationException("A node scope is required.");
        }

        if (string.IsNullOrWhiteSpace(request.RequestedBy))
        {
            throw new InvalidOperationException("RequestedBy is required for audit.");
        }

        return Task.CompletedTask;
    }
}
