namespace HostPilot.Core.Models.Mods;

public class ModImportRequest
{
    public string ServerName { get; set; } = "";
    public string ServerDirectory { get; set; } = "";
    public string SourcePath { get; set; } = "";
    public ModImportSourceType SourceType { get; set; } = ModImportSourceType.Unknown;
    public string? ProviderName { get; set; }
    public ModIntakeRules Rules { get; set; } = new();
    public string? StagingDirectory { get; set; }
    public bool OverwriteExisting { get; set; }
}
