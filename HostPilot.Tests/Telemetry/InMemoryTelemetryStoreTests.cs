using HostPilot.Telemetry.Contracts;
using HostPilot.Telemetry.Services;
using Xunit;

namespace HostPilot.Tests.Telemetry;

public sealed class InMemoryTelemetryStoreTests
{
    private readonly InMemoryTelemetryStore _store = new();

    [Fact]
    public async Task AppendNodeMetric_And_GetLatest_ReturnsOnePerNode()
    {
        var ts = DateTimeOffset.UtcNow;
        await _store.AppendNodeMetricAsync(new NodeMetricPoint("node-1", ts, 10, 20, 30, 1, 1, 2, 0), CancellationToken.None);
        await _store.AppendNodeMetricAsync(new NodeMetricPoint("node-1", ts.AddSeconds(1), 50, 60, 70, 2, 2, 2, 1), CancellationToken.None);
        await _store.AppendNodeMetricAsync(new NodeMetricPoint("node-2", ts, 5, 10, 15, 0, 0, 1, 0), CancellationToken.None);

        var result = await _store.GetLatestNodeMetricsAsync(CancellationToken.None);

        // One entry per node
        Assert.Equal(2, result.Count);
        // For node-1, the most recent metric should be returned
        var node1 = result.Single(x => x.NodeId == "node-1");
        Assert.Equal(50, node1.CpuUsagePercent);
    }

    [Fact]
    public async Task AppendHealthSnapshot_And_GetLatest_ReturnsOnePerWorkload()
    {
        var ts = DateTimeOffset.UtcNow;
        await _store.AppendHealthSnapshotAsync(new WorkloadHealthSnapshot("node-1", "wl-1", "Running", "OK", ts), CancellationToken.None);
        await _store.AppendHealthSnapshotAsync(new WorkloadHealthSnapshot("node-1", "wl-1", "Degraded", "Slow", ts.AddSeconds(5)), CancellationToken.None);
        await _store.AppendHealthSnapshotAsync(new WorkloadHealthSnapshot("node-1", "wl-2", "Running", "OK", ts), CancellationToken.None);

        var result = await _store.GetLatestHealthSnapshotsAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        // wl-1 should show the latest entry
        var wl1 = result.Single(x => x.WorkloadId == "wl-1");
        Assert.Equal("Degraded", wl1.Status);
    }

    [Fact]
    public async Task GetLatestNodeMetrics_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _store.GetLatestNodeMetricsAsync(CancellationToken.None);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestHealthSnapshots_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _store.GetLatestHealthSnapshotsAsync(CancellationToken.None);
        Assert.Empty(result);
    }
}
