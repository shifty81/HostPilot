namespace HostPilot.Core.Models.Mods;

public class ModInstallPlan
{
    public string ServerName { get; set; } = "";
    public string ServerDirectory { get; set; } = "";
    public List<ModInstallPlanItem> Items { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool RequiresRestart { get; set; }
    public bool HasBlockingIssues => Items.Any(i => i.Action == ModInstallAction.Reject);
}

public class ModInstallPlanItem
{
    public string SourcePath { get; set; } = "";
    public string DestinationPath { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public ModInstallAction Action { get; set; }
    public bool OverwriteExisting { get; set; }
    public List<string> Warnings { get; set; } = new();
}
