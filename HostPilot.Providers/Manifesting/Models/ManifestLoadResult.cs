using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Models;

public sealed class ManifestLoadResult
{
    public required ProviderManifest Manifest { get; init; }
    public List<string> Warnings { get; init; } = [];
}
