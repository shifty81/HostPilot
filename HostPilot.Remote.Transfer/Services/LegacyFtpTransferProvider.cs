namespace HostPilot.Remote.Transfer.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

public sealed class LegacyFtpTransferProvider : IRemoteFileTransferProvider
{
    public string ProviderKey => "ftp-legacy";
    public bool IsSecureByDefault => false;

    public Task CreateDirectoryAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Legacy FTP should remain disabled by default.");

    public Task DeleteAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Legacy FTP should remain disabled by default.");

    public Task<IReadOnlyList<RemoteFileEntry>> ListAsync(string nodeId, string remotePath, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Legacy FTP should remain disabled by default.");

    public Task<RemoteTransferSession> UploadAsync(RemoteTransferRequest request, Stream source, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Legacy FTP should remain disabled by default.");

    public Task<RemoteTransferSession> DownloadAsync(RemoteTransferRequest request, Stream destination, CancellationToken cancellationToken = default)
        => throw new NotSupportedException("Legacy FTP should remain disabled by default.");
}
