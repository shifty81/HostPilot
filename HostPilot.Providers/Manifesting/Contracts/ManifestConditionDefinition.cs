using HostPilot.Providers.Manifesting.Enums;

namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestConditionDefinition
{
    public required string Path { get; init; }
    public required ManifestConditionOperator Operator { get; init; }
    public object? Value { get; init; }
}
