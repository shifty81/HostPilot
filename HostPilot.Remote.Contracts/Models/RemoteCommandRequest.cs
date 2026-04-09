namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteCommandRequest
{
    public string CommandId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTimeOffset RequestedAtUtc { get; set; }
    public IReadOnlyDictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();
}
