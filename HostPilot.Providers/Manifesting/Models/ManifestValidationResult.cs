namespace HostPilot.Providers.Manifesting.Models;

public sealed class ManifestValidationResult
{
    public bool IsValid => Issues.All(x => !string.Equals(x.Severity, "Error", StringComparison.OrdinalIgnoreCase));
    public List<ManifestValidationIssue> Issues { get; init; } = [];
}
