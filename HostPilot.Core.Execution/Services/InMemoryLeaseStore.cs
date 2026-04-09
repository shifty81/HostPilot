using System.Collections.Concurrent;
using HostPilot.Core.Execution.Contracts;

namespace HostPilot.Core.Execution.Services;

public sealed class InMemoryLeaseStore : ILeaseStore
{
    private readonly ConcurrentDictionary<string, ExecutionLease> _leases = new();
    private readonly TimeSpan _defaultLeaseDuration = TimeSpan.FromMinutes(2);

    public Task<ExecutionLease?> TryAcquireLeaseAsync(
        ExecutionPlan plan,
        ExecutionWorkItem workItem,
        string nodeId,
        int attempt,
        CancellationToken cancellationToken)
    {
        var key = $"{plan.PlanId}:{workItem.WorkItemId}";
        var now = DateTimeOffset.UtcNow;

        if (_leases.TryGetValue(key, out var existing) && existing.ExpiresUtc > now)
            return Task.FromResult<ExecutionLease?>(null);

        var lease = new ExecutionLease(
            Guid.NewGuid().ToString("N"),
            plan.PlanId,
            workItem.WorkItemId,
            nodeId,
            now,
            now.Add(_defaultLeaseDuration),
            attempt);

        _leases[key] = lease;
        return Task.FromResult<ExecutionLease?>(lease);
    }

    public Task RenewLeaseAsync(ExecutionLease lease, CancellationToken cancellationToken)
    {
        var key = $"{lease.PlanId}:{lease.WorkItemId}";
        _leases[key] = lease with { ExpiresUtc = DateTimeOffset.UtcNow.Add(_defaultLeaseDuration) };
        return Task.CompletedTask;
    }

    public Task ReleaseLeaseAsync(string leaseId, CancellationToken cancellationToken)
    {
        foreach (var kvp in _leases)
        {
            if (kvp.Value.LeaseId == leaseId)
            {
                _leases.TryRemove(kvp.Key, out _);
                break;
            }
        }

        return Task.CompletedTask;
    }
}
