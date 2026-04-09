namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestRegistryEntry
{
    public required string Id { get; init; }
    public required string ManifestPath { get; init; }
    public string? AdapterType { get; init; }
    public bool Enabled { get; init; } = true;
    public List<string> Tags { get; init; } = [];
}
