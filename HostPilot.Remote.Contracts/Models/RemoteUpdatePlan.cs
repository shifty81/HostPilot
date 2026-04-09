namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteUpdatePlan
{
    public string PlanId { get; set; } = Guid.NewGuid().ToString("N");
    public string Channel { get; set; } = "stable";
    public string[] NodeIds { get; set; } = Array.Empty<string>();
    public string RolloutMode { get; set; } = "Single";
    public bool RequireApproval { get; set; } = true;
}
