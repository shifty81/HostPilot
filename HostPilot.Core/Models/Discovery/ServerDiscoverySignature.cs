namespace HostPilot.Core.Models.Discovery;

public sealed class ServerDiscoverySignature
{
    public string ServerType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> ExecutableNames { get; set; } = new();
    public List<string> RequiredFiles { get; set; } = new();
    public List<string> OptionalFiles { get; set; } = new();
    public List<string> RelativeFolders { get; set; } = new();
    public List<string> ConfigCandidates { get; set; } = new();
    public List<string> SaveCandidates { get; set; } = new();
}
