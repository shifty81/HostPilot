namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestConditionGroup
{
    public string Logic { get; init; } = "And";
    public List<ManifestConditionDefinition> Conditions { get; init; } = [];
    public List<ManifestConditionGroup> Groups { get; init; } = [];
}
