namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteCommandExecutionRecord
{
    public Guid ExecutionId { get; set; } = Guid.NewGuid();
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
    public string NodeId { get; set; } = string.Empty;
    public string CommandKey { get; set; } = string.Empty;
    public DateTimeOffset StartedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedUtc { get; set; }
    public string Status { get; set; } = "Queued";
    public string RequestedBy { get; set; } = string.Empty;
}
