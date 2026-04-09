using System.Collections.Concurrent;
using HostPilot.Security.Contracts;

namespace HostPilot.Security.Services;

public sealed class InMemoryNodeTrustStore : INodeTrustStore
{
    private readonly ConcurrentDictionary<string, NodeTrustRecord> _store = new();

    public Task<NodeTrustRecord?> GetAsync(string nodeId, CancellationToken cancellationToken)
    {
        _store.TryGetValue(nodeId, out var record);
        return Task.FromResult(record);
    }

    public Task SaveAsync(NodeTrustRecord record, CancellationToken cancellationToken)
    {
        _store[record.NodeId] = record;
        return Task.CompletedTask;
    }

    public Task RevokeAsync(string nodeId, CancellationToken cancellationToken)
    {
        if (_store.TryGetValue(nodeId, out var record))
            _store[nodeId] = record with { IsRevoked = true };

        return Task.CompletedTask;
    }
}
