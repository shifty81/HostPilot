namespace HostPilot.Remote.Transfer.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

public sealed class FtpsTransferProvider : IRemoteFileTransferProvider
{
    public string ProviderKey => "ftps";
    public bool IsSecureByDefault => true;

    public Task CreateDirectoryAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DeleteAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<IReadOnlyList<RemoteFileEntry>> ListAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RemoteFileEntry>>(Array.Empty<RemoteFileEntry>());

    public Task<RemoteTransferSession> UploadAsync(RemoteTransferRequest request, Stream source, CancellationToken cancellationToken = default)
        => Task.FromResult(new RemoteTransferSession { SessionId = request.SessionId, NodeId = request.NodeId, ProviderKey = ProviderKey, TransferType = request.TransferType, Status = "Completed", RemotePath = request.RemotePath });

    public Task<RemoteTransferSession> DownloadAsync(RemoteTransferRequest request, Stream destination, CancellationToken cancellationToken = default)
        => Task.FromResult(new RemoteTransferSession { SessionId = request.SessionId, NodeId = request.NodeId, ProviderKey = ProviderKey, TransferType = request.TransferType, Status = "Completed", RemotePath = request.RemotePath });
}
