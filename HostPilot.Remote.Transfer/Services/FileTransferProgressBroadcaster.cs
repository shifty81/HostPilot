namespace HostPilot.Remote.Transfer.Services;

using HostPilot.Remote.Contracts.Models;

public sealed class FileTransferProgressBroadcaster
{
    public event EventHandler<RemoteTransferProgress>? ProgressChanged;

    public void Publish(RemoteTransferProgress progress) => ProgressChanged?.Invoke(this, progress);
}
