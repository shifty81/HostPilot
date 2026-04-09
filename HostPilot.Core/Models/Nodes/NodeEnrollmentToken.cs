namespace HostPilot.Core.Models.Nodes;

public sealed class NodeEnrollmentToken
{
    public Guid NodeId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresUtc { get; set; }
    public bool Consumed { get; set; }
}
