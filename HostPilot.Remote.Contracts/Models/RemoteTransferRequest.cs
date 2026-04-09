namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteTransferRequest
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString("N");
    public string NodeId { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = "sftp";
    public string LocalPath { get; set; } = string.Empty;
    public string RemotePath { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public bool Overwrite { get; set; }
    public string TransferType { get; set; } = "Upload";
}
