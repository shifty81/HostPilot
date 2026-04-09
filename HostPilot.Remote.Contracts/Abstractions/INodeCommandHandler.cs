using HostPilot.Remote.Contracts.Models;

namespace HostPilot.Remote.Contracts.Abstractions;

public interface INodeCommandHandler
{
    string CommandType { get; }
    Task<RemoteCommandResult> ExecuteAsync(RemoteCommandRequest request, IProgress<RemoteCommandProgress> progress, CancellationToken cancellationToken);
}
