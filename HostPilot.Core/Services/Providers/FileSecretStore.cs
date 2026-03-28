using System.Text.Json;

namespace HostPilot.Core.Services.Providers;

public sealed class FileSecretStore : ISecretStore
{
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public FileSecretStore(string path)
    {
        _path = path;
    }

    public async Task<string?> GetSecretAsync(string key, CancellationToken cancellationToken = default)
    {
        var values = await LoadAsync(cancellationToken).ConfigureAwait(false);
        values.TryGetValue(key, out var value);
        return value;
    }

    public async Task SetSecretAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var values = await LoadAsync(cancellationToken).ConfigureAwait(false);
        values[key] = value;
        await SaveAsync(values, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveSecretAsync(string key, CancellationToken cancellationToken = default)
    {
        var values = await LoadAsync(cancellationToken).ConfigureAwait(false);
        if (values.Remove(key))
        {
            await SaveAsync(values, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<Dictionary<string, string>> LoadAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!File.Exists(_path))
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            await using var stream = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream, cancellationToken: cancellationToken).ConfigureAwait(false)
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task SaveAsync(Dictionary<string, string> values, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var stream = File.Create(_path);
            await JsonSerializer.SerializeAsync(stream, values, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }
}
