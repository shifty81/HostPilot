using System.Text.Json;
using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ProviderManifestRegistry : IProviderManifestRegistry
{
    private readonly string _registryFilePath;

    public ProviderManifestRegistry(string registryFilePath)
    {
        _registryFilePath = registryFilePath;
    }

    public async Task<IReadOnlyList<ManifestRegistryEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(_registryFilePath);
        var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var providersElement = document.RootElement.GetProperty("providers");
        var entries = providersElement.Deserialize<List<ManifestRegistryEntry>>(JsonManifestSerializer.Options) ?? [];
        return entries;
    }

    public async Task<ManifestRegistryEntry?> FindByIdAsync(string manifestId, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.FirstOrDefault(x => string.Equals(x.Id, manifestId, StringComparison.OrdinalIgnoreCase));
    }
}
