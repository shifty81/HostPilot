using System.Collections.Generic;
using HostPilot.Core.Models.DynamicForms;

namespace HostPilot.Core.Services.DynamicForms;

public sealed class ManifestDocument
{
    public List<ManifestTabDto> Tabs { get; set; } = new();
    public List<ManifestFieldDto> Fields { get; set; } = new();
    public List<ManifestRuleDto> Rules { get; set; } = new();
}
