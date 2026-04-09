using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public interface IProviderManifestLoader
{
    Task<ManifestLoadResult> LoadAsync(string manifestId, CancellationToken cancellationToken = default);
}
