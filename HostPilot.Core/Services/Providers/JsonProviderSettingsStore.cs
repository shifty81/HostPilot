using System.Text.Json;
using HostPilot.Core.Models.Providers;

namespace HostPilot.Core.Services.Providers;

public sealed class JsonProviderSettingsStore : IProviderSettingsStore
{
    private readonly string _settingsPath;
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public JsonProviderSettingsStore(string settingsPath)
    {
        _settingsPath = settingsPath;
    }

    public async Task<ProviderSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_settingsPath))
        {
            return new ProviderSettings();
        }

        await using var stream = File.OpenRead(_settingsPath);
        return await JsonSerializer.DeserializeAsync<ProviderSettings>(stream, Options, cancellationToken).ConfigureAwait(false)
            ?? new ProviderSettings();
    }

    public async Task SaveAsync(ProviderSettings settings, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_settingsPath);
        await JsonSerializer.SerializeAsync(stream, settings, Options, cancellationToken).ConfigureAwait(false);
    }
}
