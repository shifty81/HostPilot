using HostPilot.Core.Models.Nodes;

namespace HostPilot.Core.Services.Nodes;

public interface INodeRegistryService
{
    Task<IReadOnlyList<ManagedNode>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ManagedNode?> GetAsync(Guid nodeId, CancellationToken cancellationToken = default);
    Task<ManagedNode> UpsertAsync(ManagedNode node, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid nodeId, CancellationToken cancellationToken = default);
    Task UpdateHeartbeatAsync(NodeHeartbeat heartbeat, CancellationToken cancellationToken = default);
}
