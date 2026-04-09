using HostPilot.Providers.Manifesting.Enums;

namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestFieldDefinition
{
    public required string Id { get; init; }
    public required string SectionId { get; init; }
    public required string Key { get; init; }
    public required ManifestFieldType Type { get; init; }
    public required string Label { get; init; }
    public string? Description { get; init; }
    public string? Placeholder { get; init; }
    public object? Default { get; init; }
    public bool Required { get; init; }
    public bool Advanced { get; init; }
    public bool ReadOnly { get; init; }
    public int Order { get; init; }
    public int ColumnSpan { get; init; } = 1;
    public double? Min { get; init; }
    public double? Max { get; init; }
    public double? Step { get; init; }
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public string? RegexPattern { get; init; }
    public List<ManifestFieldOptionDefinition> Options { get; init; } = [];
    public List<ManifestValidationDefinition> Validation { get; init; } = [];
    public ManifestConditionGroup? VisibleWhen { get; init; }
    public ManifestConditionGroup? EnabledWhen { get; init; }
    public ManifestConditionGroup? RequiredWhen { get; init; }
    public ManifestFieldOutputDefinition Output { get; init; } = new();
}
