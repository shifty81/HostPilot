namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteUpdateProgress
{
    public string PlanId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool? HealthyAfterApply { get; set; }
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
}
