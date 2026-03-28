namespace HostPilot.Core.Models.Mods;

public class ModImportCandidate
{
    public string DisplayName { get; set; } = "";
    public string SourcePath { get; set; } = "";
    public string RelativeSourcePath { get; set; } = "";
    public ModImportSourceType SourceType { get; set; }
    public ModPackageType PackageType { get; set; }
    public string? DetectedExtension { get; set; }
    public string? SuggestedTargetRelativePath { get; set; }
    public bool RequiresExtraction { get; set; }
    public bool RequiresReview { get; set; }
    public bool IsDuplicate { get; set; }
    public string? ExistingInstalledPath { get; set; }
    public string? DetectedLoader { get; set; }
    public string? DetectedVersion { get; set; }
    public string? HashSha256 { get; set; }
    public ModCompatibilityLevel Compatibility { get; set; } = ModCompatibilityLevel.Unknown;
    public List<string> Warnings { get; set; } = new();
    public List<string> DetectedDependencies { get; set; } = new();
}
