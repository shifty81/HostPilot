namespace HostPilot.Providers.Manifesting.Models;

public sealed class ManifestValidationIssue
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public required string Severity { get; init; }
    public string? FieldId { get; init; }
    public string? ConfigKey { get; init; }
    public string? TabId { get; init; }
    public string? SectionId { get; init; }
}
