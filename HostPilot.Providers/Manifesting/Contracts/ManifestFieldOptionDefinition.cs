namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestFieldOptionDefinition
{
    public required string Value { get; init; }
    public required string Label { get; init; }
    public string? Description { get; init; }
    public bool Disabled { get; init; }
}
