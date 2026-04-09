namespace HostPilot.Web.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

public sealed class NullRemoteUpdatePackageSource : IRemoteUpdatePackageSource
{
    public Task<RemoteUpdatePackageManifest?> GetLatestAsync(string channel, string targetRuntime, CancellationToken cancellationToken = default)
        => Task.FromResult<RemoteUpdatePackageManifest?>(null);

    public Task<Stream> OpenPackageAsync(RemoteUpdatePackageManifest manifest, CancellationToken cancellationToken = default)
        => Task.FromResult<Stream>(Stream.Null);
}
