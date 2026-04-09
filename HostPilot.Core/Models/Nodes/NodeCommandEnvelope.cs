namespace HostPilot.Core.Models.Nodes;

public sealed class NodeCommandEnvelope
{
    public Guid CommandId { get; set; } = Guid.NewGuid();
    public Guid NodeId { get; set; }
    public string CommandType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClaimedUtc { get; set; }
    public DateTimeOffset? CompletedUtc { get; set; }
    public string Status { get; set; } = "Pending";
    public string? ResultJson { get; set; }
    public string? Error { get; set; }
}
