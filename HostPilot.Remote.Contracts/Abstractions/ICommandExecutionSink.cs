namespace HostPilot.Remote.Contracts.Abstractions;

using HostPilot.Remote.Contracts.Models;

public interface ICommandExecutionSink
{
    Task PublishProgressAsync(RemoteExecutionProgress progress, CancellationToken cancellationToken = default);
    Task PublishCompletedAsync(RemoteExecutionResult result, CancellationToken cancellationToken = default);
}
