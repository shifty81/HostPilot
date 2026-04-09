namespace HostPilot.Core.Models.DynamicForms;

public sealed class ManifestRuleDto
{
    public string Action { get; set; } = string.Empty;
    public string TargetKey { get; set; } = string.Empty;
    public string SourceKey { get; set; } = string.Empty;
    public new string? Equals { get; set; }
}
