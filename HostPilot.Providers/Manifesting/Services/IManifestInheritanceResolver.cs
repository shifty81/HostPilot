using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Services;

public interface IManifestInheritanceResolver
{
    Task<ProviderManifest> ResolveAsync(ProviderManifest manifest, CancellationToken cancellationToken = default);
}
