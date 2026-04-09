using HostPilot.Providers.Manifesting.Enums;

namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestOutputTargetDefinition
{
    public required ManifestOutputTargetType Type { get; init; }
    public string? File { get; init; }
    public string? Path { get; init; }
    public string? ArgName { get; init; }
    public string? EnvironmentVariable { get; init; }
    public string? Format { get; init; }
    public bool OmitWhenEmpty { get; init; } = true;
}
