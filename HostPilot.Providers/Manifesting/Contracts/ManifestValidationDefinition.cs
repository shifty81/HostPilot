using HostPilot.Providers.Manifesting.Enums;

namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestValidationDefinition
{
    public required ManifestValidationType Type { get; init; }
    public string? Message { get; init; }
    public string? Pattern { get; init; }
    public double? Min { get; init; }
    public double? Max { get; init; }
    public List<string> AllowedValues { get; init; } = [];
    public string? CompareToKey { get; init; }
}
