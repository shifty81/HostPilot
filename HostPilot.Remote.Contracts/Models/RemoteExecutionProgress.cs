namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteExecutionProgress
{
    public string CorrelationId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string CommandKey { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int? Percent { get; set; }
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
}
