namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteExecutionRequest
{
    public string NodeId { get; set; } = string.Empty;
    public string CommandKey { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public Dictionary<string, string> Arguments { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? ServerId { get; set; }
    public string? CorrelationId { get; set; }
}
