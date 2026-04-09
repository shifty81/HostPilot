using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ManifestDefaultResolver : IManifestDefaultResolver
{
    public IReadOnlyDictionary<string, object?> BuildDefaultState(ProviderManifest manifest, string? presetId = null)
    {
        var state = new Dictionary<string, object?>(manifest.Defaults, StringComparer.OrdinalIgnoreCase);

        foreach (var field in manifest.Fields)
        {
            if (!state.ContainsKey(field.Key) && field.Default is not null)
            {
                state[field.Key] = field.Default;
            }
        }

        if (!string.IsNullOrWhiteSpace(presetId))
        {
            var preset = manifest.Presets.FirstOrDefault(x => string.Equals(x.Id, presetId, StringComparison.OrdinalIgnoreCase));
            if (preset is not null)
            {
                foreach (var pair in preset.Overrides)
                {
                    state[pair.Key] = pair.Value;
                }
            }
        }

        return state;
    }
}
