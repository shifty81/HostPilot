namespace HostPilot.Core.Models.Mods;

public sealed class ModProviderCapabilities
{
    public bool SupportsSearch { get; set; }
    public bool SupportsDependencies { get; set; }
    public bool SupportsVersionFiltering { get; set; }
    public bool SupportsCategories { get; set; }
    public bool SupportsServerOnlyFiltering { get; set; }
}
