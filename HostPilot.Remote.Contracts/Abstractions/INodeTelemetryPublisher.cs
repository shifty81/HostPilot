using HostPilot.Remote.Contracts.Models;

namespace HostPilot.Remote.Contracts.Abstractions;

public interface INodeTelemetryPublisher
{
    Task PublishHeartbeatAsync(NodeHeartbeat heartbeat, CancellationToken cancellationToken);
    Task PublishProgressAsync(RemoteCommandProgress progress, CancellationToken cancellationToken);
    Task PublishResultAsync(RemoteCommandResult result, CancellationToken cancellationToken);
}
