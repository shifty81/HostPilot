using System.Collections.Generic;

namespace HostPilot.Core.Models.DynamicForms;

public sealed class ManifestFieldDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "Text";
    public string TabKey { get; set; } = "general";
    public string Group { get; set; } = "General";
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public object? DefaultValue { get; set; }
    public object? Min { get; set; }
    public object? Max { get; set; }
    public string? Placeholder { get; set; }
    public string? BrowseMode { get; set; }
    public string? ValidationRegex { get; set; }
    public string? FileFilter { get; set; }
    public List<FieldOption>? Options { get; set; }
}
