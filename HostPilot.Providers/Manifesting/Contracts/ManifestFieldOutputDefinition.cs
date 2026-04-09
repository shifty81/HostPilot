namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestFieldOutputDefinition
{
    public List<ManifestOutputTargetDefinition> Targets { get; init; } = [];
}
