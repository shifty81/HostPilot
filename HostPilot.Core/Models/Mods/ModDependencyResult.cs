namespace HostPilot.Core.Models.Mods;

public sealed class ModDependencyResult
{
    public List<ModPackage> ResolvedPackages { get; set; } = new();
    public List<string> MissingDependencies { get; set; } = new();
    public List<string> Conflicts { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
