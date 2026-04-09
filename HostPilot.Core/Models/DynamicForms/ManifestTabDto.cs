namespace HostPilot.Core.Models.DynamicForms;

public sealed class ManifestTabDto
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
}
