using HostPilot.Telemetry.Contracts;

namespace HostPilot.Telemetry.Services;

public sealed class TelemetryIngestService
{
    private readonly ITelemetryStore _store;
    private readonly IAlertSink _alerts;
    private readonly ITelemetryBroadcaster _broadcaster;

    public TelemetryIngestService(
        ITelemetryStore store,
        IAlertSink alerts,
        ITelemetryBroadcaster broadcaster)
    {
        _store = store;
        _alerts = alerts;
        _broadcaster = broadcaster;
    }

    public async Task IngestNodeMetricAsync(NodeMetricPoint point, CancellationToken cancellationToken)
    {
        await _store.AppendNodeMetricAsync(point, cancellationToken);

        if (point.CpuUsagePercent >= 95 || point.MemoryUsagePercent >= 95 || point.DiskUsagePercent >= 95)
        {
            var alert = new ClusterAlert(
                Guid.NewGuid().ToString("N"),
                "Warning",
                "Capacity",
                $"Node '{point.NodeId}' is approaching a resource ceiling.",
                DateTimeOffset.UtcNow,
                new Dictionary<string, string>
                {
                    ["cpu"] = point.CpuUsagePercent.ToString("F2"),
                    ["memory"] = point.MemoryUsagePercent.ToString("F2"),
                    ["disk"] = point.DiskUsagePercent.ToString("F2")
                });

            await _alerts.PublishAsync(alert, cancellationToken);
            await _broadcaster.BroadcastAsync("alerts", alert, cancellationToken);
        }

        await _broadcaster.BroadcastAsync("node-metrics", point, cancellationToken);
    }

    public async Task<ClusterCapacitySnapshot> BuildCapacitySnapshotAsync(CancellationToken cancellationToken)
    {
        var latest = await _store.GetLatestNodeMetricsAsync(cancellationToken);
        var online = latest.Count;

        // Placeholder values — replace with real node inventory in final integration.
        var totalCpu = latest.Sum(x => 16);
        var totalMemory = latest.Sum(x => 32768);
        var usedCpu = latest.Sum(x => (int)Math.Round(16 * (x.CpuUsagePercent / 100.0)));
        var usedMemory = latest.Sum(x => (int)Math.Round(32768 * (x.MemoryUsagePercent / 100.0)));

        return new ClusterCapacitySnapshot(
            DateTimeOffset.UtcNow,
            online,
            totalCpu,
            Math.Max(0, totalCpu - usedCpu),
            totalMemory,
            Math.Max(0, totalMemory - usedMemory),
            latest.Sum(x => x.RunningJobs));
    }
}
