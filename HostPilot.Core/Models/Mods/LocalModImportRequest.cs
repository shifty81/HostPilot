namespace HostPilot.Core.Models.Mods;

public sealed class LocalModImportRequest
{
    public Guid DeploymentId { get; set; }
    public string GameId { get; set; } = string.Empty;
    public IReadOnlyList<string> SourcePaths { get; set; } = Array.Empty<string>();
    public bool CopyIntoManagedCache { get; set; } = true;
}
