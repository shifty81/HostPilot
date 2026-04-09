using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public interface IManifestValidationEngine
{
    ManifestValidationResult Validate(ProviderManifest manifest, IReadOnlyDictionary<string, object?> state, bool isAdvancedMode = false);
}
