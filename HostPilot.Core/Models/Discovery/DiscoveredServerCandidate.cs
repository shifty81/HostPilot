namespace HostPilot.Core.Models.Discovery;

public sealed class DiscoveredServerCandidate
{
    public string CandidateId { get; set; } = Guid.NewGuid().ToString("N");
    /// <summary>Deployment manifest ID (e.g. "valheim", "minecraft-java").</summary>
    public string ManifestId { get; set; } = string.Empty;
    /// <summary>Server type identifier. Mirrors <see cref="ManifestId"/> for manifest-backed candidates.</summary>
    public string ServerType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string InstallPath { get; set; } = string.Empty;
    public string ExecutablePath { get; set; } = string.Empty;
    public string? ConfigPath { get; set; }
    public string? SavePath { get; set; }
    public double Confidence { get; set; }
    public bool IsImported { get; set; }
    public bool IsExternalManaged { get; set; } = true;
    public List<string> Evidence { get; set; } = new();
    public List<string> SuggestedConfigFiles { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
