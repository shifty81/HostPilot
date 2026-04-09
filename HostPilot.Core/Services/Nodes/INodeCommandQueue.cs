using HostPilot.Core.Models.Nodes;

namespace HostPilot.Core.Services.Nodes;

public interface INodeCommandQueue
{
    Task<NodeCommandEnvelope> EnqueueAsync(NodeCommandEnvelope command, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NodeCommandEnvelope>> DequeuePendingAsync(Guid nodeId, int maxCount, CancellationToken cancellationToken = default);
    Task CompleteAsync(Guid commandId, string resultJson, CancellationToken cancellationToken = default);
    Task FailAsync(Guid commandId, string error, CancellationToken cancellationToken = default);
}
