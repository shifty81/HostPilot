using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Services;

public interface IProviderManifestRegistry
{
    Task<IReadOnlyList<ManifestRegistryEntry>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ManifestRegistryEntry?> FindByIdAsync(string manifestId, CancellationToken cancellationToken = default);
}
