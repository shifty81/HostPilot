namespace HostPilot.Core.Models.FirstRun;

public sealed class FirstRunWizardState
{
    public bool HasCompletedWizard { get; set; }
    public bool HasConfiguredSteamCmd { get; set; }
    public bool HasCompletedDiscoveryScan { get; set; }
    public DateTimeOffset? LastCompletedAtUtc { get; set; }
    public DateTimeOffset? LastDiscoveryScanAtUtc { get; set; }
    public List<string> ImportedCandidateIds { get; set; } = new();
}
