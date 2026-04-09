namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ProviderCapabilitySet
{
    public bool SupportsInstall { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsImport { get; init; }
    public bool SupportsStart { get; init; } = true;
    public bool SupportsStop { get; init; } = true;
    public bool SupportsValidation { get; init; } = true;
    public bool SupportsRcon { get; init; }
    public bool SupportsQuery { get; init; }
    public bool SupportsMods { get; init; }
    public bool SupportsWorkshop { get; init; }
    public bool SupportsCluster { get; init; }
    public bool SupportsBackups { get; init; }
    public bool SupportsProfiles { get; init; }
    public bool SupportsConfigFiles { get; init; } = true;
    public bool SupportsCustomStartArgs { get; init; } = true;
    public bool SupportsMultipleInstances { get; init; } = true;
    public bool SupportsSaveDiscovery { get; init; }
}
