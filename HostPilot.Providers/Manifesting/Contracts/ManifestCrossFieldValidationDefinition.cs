namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestCrossFieldValidationDefinition
{
    public required string Id { get; init; }
    public required string Rule { get; init; }
    public required string Message { get; init; }
    public string Severity { get; init; } = "Error";
}
