namespace HostPilot.Core.Models.Mods;

public sealed class ModPackage
{
    public string ProviderId { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public bool ServerSideSupported { get; set; } = true;
    public List<string> Dependencies { get; set; } = new();
    public List<string> Conflicts { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();
}
