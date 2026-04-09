namespace HostPilot.Remote.Transfer.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

public sealed class SftpTransferProvider : IRemoteFileTransferProvider
{
    public string ProviderKey => "sftp";
    public bool IsSecureByDefault => true;

    public Task CreateDirectoryAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DeleteAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<IReadOnlyList<RemoteFileEntry>> ListAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RemoteFileEntry>>(new[] { new RemoteFileEntry { Name = "server", FullPath = remotePath, IsDirectory = true } });

    public Task<RemoteTransferSession> UploadAsync(RemoteTransferRequest request, Stream source, CancellationToken cancellationToken = default)
        => Task.FromResult(new RemoteTransferSession { SessionId = request.SessionId, NodeId = request.NodeId, ProviderKey = ProviderKey, TransferType = request.TransferType, Status = "Completed", RemotePath = request.RemotePath });

    public Task<RemoteTransferSession> DownloadAsync(RemoteTransferRequest request, Stream destination, CancellationToken cancellationToken = default)
        => Task.FromResult(new RemoteTransferSession { SessionId = request.SessionId, NodeId = request.NodeId, ProviderKey = ProviderKey, TransferType = request.TransferType, Status = "Completed", RemotePath = request.RemotePath });
}
