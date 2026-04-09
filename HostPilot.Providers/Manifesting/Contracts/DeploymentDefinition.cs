namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class DeploymentDefinition
{
    public string? InstallStrategy { get; init; }
    public int? SteamAppId { get; init; }
    public bool AnonymousLogin { get; init; } = true;
    public string? DownloadSource { get; init; }
    public string? DefaultInstallSubpath { get; init; }
    public string? DefaultExecutable { get; init; }
    public string? WorkingDirectory { get; init; }
    public string? StartupMode { get; init; }
    public string? ConfigStorageMode { get; init; }
    public List<string> RuntimeDependencyIds { get; init; } = [];
}
