namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteTransferSession
{
    public string SessionId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string TransferType { get; set; } = string.Empty;
    public string Status { get; set; } = "Queued";
    public string RemotePath { get; set; } = string.Empty;
    public string? ChecksumSha256 { get; set; }
}
