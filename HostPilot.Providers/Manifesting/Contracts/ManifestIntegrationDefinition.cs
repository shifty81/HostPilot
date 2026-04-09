namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestIntegrationDefinition
{
    public ManifestModsIntegrationDefinition Mods { get; init; } = new();
}

public sealed class ManifestModsIntegrationDefinition
{
    public string Mode { get; init; } = "None";
    public string? Provider { get; init; }
    public bool SupportsCollections { get; init; }
    public bool SupportsDependencyResolution { get; init; }
    public bool SupportsZipImport { get; init; }
    public bool SupportsFolderImport { get; init; }
    public bool SupportsCatalogBrowser { get; init; }
}
