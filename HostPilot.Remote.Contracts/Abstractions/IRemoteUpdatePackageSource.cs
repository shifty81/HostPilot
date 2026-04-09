namespace HostPilot.Remote.Contracts.Abstractions;

using HostPilot.Remote.Contracts.Models;

public interface IRemoteUpdatePackageSource
{
    Task<RemoteUpdatePackageManifest?> GetLatestAsync(string channel, string targetRuntime, CancellationToken cancellationToken = default);
    Task<Stream> OpenPackageAsync(RemoteUpdatePackageManifest manifest, CancellationToken cancellationToken = default);
}
