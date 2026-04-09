namespace HostPilot.Remote.Execution.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

public sealed class NullCommandExecutionSink : ICommandExecutionSink
{
    public Task PublishProgressAsync(RemoteExecutionProgress progress, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task PublishCompletedAsync(RemoteExecutionResult result, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
