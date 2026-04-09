using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Models;

public sealed class ManifestEvaluationContext
{
    public required ProviderManifest Manifest { get; init; }
    public required IReadOnlyDictionary<string, object?> State { get; init; }
    public bool IsAdvancedMode { get; init; }
}
