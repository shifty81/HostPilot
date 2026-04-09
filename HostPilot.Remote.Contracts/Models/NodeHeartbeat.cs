namespace HostPilot.Remote.Contracts.Models;

public sealed class NodeHeartbeat
{
    public string NodeId { get; set; } = string.Empty;
    public DateTimeOffset SentAtUtc { get; set; }
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public int RunningServerCount { get; set; }
    public int RunningOperationCount { get; set; }
    public IReadOnlyList<NodeAlertSnapshot> Alerts { get; set; } = Array.Empty<NodeAlertSnapshot>();
}
