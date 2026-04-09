using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public interface IManifestOutputMapper
{
    ManifestOutputMap BuildOutput(ProviderManifest manifest, IReadOnlyDictionary<string, object?> state);
}
