namespace HostPilot.Core.Models.Mods;

public sealed class DependencyReviewItem
{
    public string DependencyId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsAlreadyInstalled { get; set; }
    public bool IsSelected { get; set; } = true;
    public string Reason { get; set; } = string.Empty;
}
