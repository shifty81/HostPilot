namespace HostPilot.Core.Models.Templates;

public sealed class TemplateModReference
{
    public string ProviderId { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public bool Required { get; set; } = true;
    public string? Version { get; set; }
}
