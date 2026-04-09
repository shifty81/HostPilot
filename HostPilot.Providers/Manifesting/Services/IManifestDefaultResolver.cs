using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Services;

public interface IManifestDefaultResolver
{
    IReadOnlyDictionary<string, object?> BuildDefaultState(ProviderManifest manifest, string? presetId = null);
}
