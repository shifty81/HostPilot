using HostPilot.Telemetry.Contracts;

namespace HostPilot.Telemetry.Services;

public sealed class InMemoryTelemetryStore : ITelemetryStore
{
    private readonly List<NodeMetricPoint> _metrics = new();
    private readonly List<WorkloadHealthSnapshot> _health = new();
    private readonly object _gate = new();

    public Task AppendNodeMetricAsync(NodeMetricPoint point, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            _metrics.Add(point);
        }
        return Task.CompletedTask;
    }

    public Task AppendHealthSnapshotAsync(WorkloadHealthSnapshot snapshot, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            _health.Add(snapshot);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<NodeMetricPoint>> GetLatestNodeMetricsAsync(CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            return Task.FromResult<IReadOnlyList<NodeMetricPoint>>(
                _metrics.GroupBy(x => x.NodeId).Select(g => g.OrderByDescending(x => x.TimestampUtc).First()).ToList());
        }
    }

    public Task<IReadOnlyList<WorkloadHealthSnapshot>> GetLatestHealthSnapshotsAsync(CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            return Task.FromResult<IReadOnlyList<WorkloadHealthSnapshot>>(
                _health.GroupBy(x => $"{x.NodeId}:{x.WorkloadId}").Select(g => g.OrderByDescending(x => x.TimestampUtc).First()).ToList());
        }
    }
}
