using HostPilot.Telemetry.Contracts;
using HostPilot.ViewModels;

namespace HostPilot.Services;

public sealed class TelemetryDashboardAdapter
{
    public void ApplyMetrics(
        ClusterOperationsDashboardViewModel dashboard,
        IReadOnlyList<NodeMetricPoint> points)
    {
        dashboard.OnlineNodes = points.Count;
        dashboard.RunningJobs = points.Sum(x => x.RunningJobs);
        dashboard.ClusterStatus = points.Count == 0 ? "Offline" : "Online";

        dashboard.Nodes.Clear();
        foreach (var point in points.OrderBy(x => x.NodeId))
        {
            dashboard.Nodes.Add(new NodeTileViewModel
            {
                DisplayName = point.NodeId,
                Health = point.CpuUsagePercent > 95 || point.MemoryUsagePercent > 95 ? "Warning" : "Healthy",
                Cpu = point.CpuUsagePercent,
                Memory = point.MemoryUsagePercent
            });
        }
    }
}
