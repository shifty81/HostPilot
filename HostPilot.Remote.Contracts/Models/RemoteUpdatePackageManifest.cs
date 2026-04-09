namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteUpdatePackageManifest
{
    public string PackageId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Channel { get; set; } = "stable";
    public string TargetRuntime { get; set; } = "win-x64";
    public string Sha256 { get; set; } = string.Empty;
    public string MinimumControllerVersion { get; set; } = string.Empty;
    public string? RollbackPackageId { get; set; }
    public bool IsSigned { get; set; }
}
