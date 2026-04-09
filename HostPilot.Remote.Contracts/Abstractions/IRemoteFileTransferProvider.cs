namespace HostPilot.Remote.Contracts.Abstractions;

using HostPilot.Remote.Contracts.Models;

public interface IRemoteFileTransferProvider
{
    string ProviderKey { get; }
    bool IsSecureByDefault { get; }

    Task<IReadOnlyList<RemoteFileEntry>> ListAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default);
    Task<RemoteTransferSession> UploadAsync(RemoteTransferRequest request, Stream source, CancellationToken cancellationToken = default);
    Task<RemoteTransferSession> DownloadAsync(RemoteTransferRequest request, Stream destination, CancellationToken cancellationToken = default);
    Task DeleteAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default);
    Task CreateDirectoryAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default);
}
