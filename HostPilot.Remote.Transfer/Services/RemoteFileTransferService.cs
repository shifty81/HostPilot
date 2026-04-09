namespace HostPilot.Remote.Transfer.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

public sealed class RemoteFileTransferService
{
    private readonly IReadOnlyDictionary<string, IRemoteFileTransferProvider> _providers;
    private readonly FileTransferProgressBroadcaster _broadcaster;

    public RemoteFileTransferService(IEnumerable<IRemoteFileTransferProvider> providers, FileTransferProgressBroadcaster broadcaster)
    {
        _providers = providers.ToDictionary(x => x.ProviderKey, StringComparer.OrdinalIgnoreCase);
        _broadcaster = broadcaster;
    }

    public Task<IReadOnlyList<RemoteFileEntry>> ListAsync(string nodeId, string providerKey, string remotePath, CancellationToken cancellationToken = default)
        => Resolve(providerKey).ListAsync(nodeId, remotePath, cancellationToken);

    public async Task<RemoteTransferSession> UploadAsync(RemoteTransferRequest request, Stream source, CancellationToken cancellationToken = default)
    {
        var session = await Resolve(request.ProviderKey).UploadAsync(request, source, cancellationToken);
        _broadcaster.Publish(new RemoteTransferProgress
        {
            SessionId = session.SessionId,
            NodeId = request.NodeId,
            RemotePath = request.RemotePath,
            Status = session.Status,
            Percent = 100,
            BytesTransferred = source.CanSeek ? source.Length : 0,
            TotalBytes = source.CanSeek ? source.Length : null,
        });
        return session;
    }

    public async Task<RemoteTransferSession> DownloadAsync(RemoteTransferRequest request, Stream destination, CancellationToken cancellationToken = default)
    {
        var session = await Resolve(request.ProviderKey).DownloadAsync(request, destination, cancellationToken);
        _broadcaster.Publish(new RemoteTransferProgress
        {
            SessionId = session.SessionId,
            NodeId = request.NodeId,
            RemotePath = request.RemotePath,
            Status = session.Status,
            Percent = 100,
            BytesTransferred = destination.CanSeek ? destination.Length : 0,
            TotalBytes = destination.CanSeek ? destination.Length : null,
        });
        return session;
    }

    private IRemoteFileTransferProvider Resolve(string providerKey)
    {
        if (_providers.TryGetValue(providerKey, out var provider))
        {
            return provider;
        }

        throw new KeyNotFoundException($"No file transfer provider registered for '{providerKey}'.");
    }
}
