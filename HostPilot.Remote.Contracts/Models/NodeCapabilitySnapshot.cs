namespace HostPilot.Remote.Contracts.Models;

public sealed class NodeCapabilitySnapshot
{
    public int LogicalCpuCount { get; set; }
    public long TotalMemoryBytes { get; set; }
    public long AvailableMemoryBytes { get; set; }
    public long TotalDiskBytes { get; set; }
    public long FreeDiskBytes { get; set; }
    public bool SupportsSteamCmd { get; set; }
    public bool SupportsRcon { get; set; }
    public bool SupportsWorkshopSync { get; set; }
    public bool SupportsBackups { get; set; }
    public IReadOnlyDictionary<string, string> ProviderVersions { get; set; } = new Dictionary<string, string>();
}
