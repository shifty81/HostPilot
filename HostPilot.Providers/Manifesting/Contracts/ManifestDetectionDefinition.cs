namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestDetectionDefinition
{
    public string Strategy { get; init; } = "None";
    public List<string> ExecutableNames { get; init; } = [];
    public List<string> ConfigFiles { get; init; } = [];
    public List<string> InstallMarkers { get; init; } = [];
}
