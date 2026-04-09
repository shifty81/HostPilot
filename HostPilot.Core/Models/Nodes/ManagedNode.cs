using System.Text.Json.Serialization;

namespace HostPilot.Core.Models.Nodes;

public sealed class ManagedNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string AgentVersion { get; set; } = string.Empty;
    public string ControllerUrl { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
    public DateTimeOffset? LastHeartbeatUtc { get; set; }
    public int CpuPercent { get; set; }
    public long MemoryUsedMb { get; set; }
    public long MemoryTotalMb { get; set; }
    public bool IsLocalNode { get; set; }
    public string TagsCsv { get; set; } = string.Empty;
    public List<NodeCapability> Capabilities { get; set; } = new();
    public List<NodeServerInventoryItem> Servers { get; set; } = new();

    [JsonIgnore]
    public bool IsOnline => LastHeartbeatUtc.HasValue && DateTimeOffset.UtcNow - LastHeartbeatUtc.Value < TimeSpan.FromSeconds(90);
}
