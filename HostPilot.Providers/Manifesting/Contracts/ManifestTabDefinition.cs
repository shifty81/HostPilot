namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestTabDefinition
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? IconKey { get; init; }
    public int Order { get; init; }
    public bool Advanced { get; init; }
    public ManifestConditionGroup? VisibleWhen { get; init; }
    public ManifestConditionGroup? EnabledWhen { get; init; }
}
