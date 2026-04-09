namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ProviderManifestMetadata
{
    public string? Author { get; init; }
    public string? Source { get; init; }
    public List<string> Tags { get; init; } = [];
    public bool Enabled { get; init; } = true;
    public int SortOrder { get; init; }
    public string? IconKey { get; init; }
}
