namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestPresetDefinition
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public int Order { get; init; }
    public Dictionary<string, object?> Overrides { get; init; } = new();
}
