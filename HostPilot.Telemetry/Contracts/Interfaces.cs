namespace HostPilot.Telemetry.Contracts;

public interface ITelemetryStore
{
    Task AppendNodeMetricAsync(NodeMetricPoint point, CancellationToken cancellationToken);
    Task AppendHealthSnapshotAsync(WorkloadHealthSnapshot snapshot, CancellationToken cancellationToken);
    Task<IReadOnlyList<NodeMetricPoint>> GetLatestNodeMetricsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkloadHealthSnapshot>> GetLatestHealthSnapshotsAsync(CancellationToken cancellationToken);
}

public interface IAlertSink
{
    Task PublishAsync(ClusterAlert alert, CancellationToken cancellationToken);
}

public interface ITelemetryBroadcaster
{
    Task BroadcastAsync(string topic, object payload, CancellationToken cancellationToken);
}
