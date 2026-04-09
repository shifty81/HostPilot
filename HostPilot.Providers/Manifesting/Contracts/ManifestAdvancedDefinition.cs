namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestAdvancedDefinition
{
    public bool SupportsRawLaunchArgs { get; init; }
    public bool SupportsRawConfigOverrides { get; init; }
    public bool SupportsJsonPatchOverrides { get; init; }
    public Dictionary<string, object?> Extensions { get; init; } = new();
}
