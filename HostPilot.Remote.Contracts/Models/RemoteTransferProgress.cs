namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteTransferProgress
{
    public string SessionId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string RemotePath { get; set; } = string.Empty;
    public long BytesTransferred { get; set; }
    public long? TotalBytes { get; set; }
    public int? Percent { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
}
