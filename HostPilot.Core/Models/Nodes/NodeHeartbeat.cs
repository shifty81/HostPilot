namespace HostPilot.Core.Models.Nodes;

public sealed class NodeHeartbeat
{
    public Guid NodeId { get; set; }
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
    public int CpuPercent { get; set; }
    public long MemoryUsedMb { get; set; }
    public long MemoryTotalMb { get; set; }
    public int RunningServerCount { get; set; }
    public string AgentVersion { get; set; } = string.Empty;
    public string Status { get; set; } = "Online";
}
