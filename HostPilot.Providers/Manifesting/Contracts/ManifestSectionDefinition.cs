namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestSectionDefinition
{
    public required string Id { get; init; }
    public required string TabId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public int Order { get; init; }
    public bool Advanced { get; init; }
    public bool Collapsible { get; init; } = true;
    public bool InitiallyExpanded { get; init; } = true;
    public ManifestConditionGroup? VisibleWhen { get; init; }
    public ManifestConditionGroup? EnabledWhen { get; init; }
}
