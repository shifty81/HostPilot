namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteCommandProgress
{
    public string CommandId { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Percent { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
}
