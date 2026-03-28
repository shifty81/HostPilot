using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public sealed class LocalOnlyModProvider : IModProvider
{
    public ModProviderType ProviderType => ModProviderType.Local;

    public ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = false,
        SupportsDependencies = false,
        SupportsVersionFiltering = false,
        SupportsCategories = false,
        SupportsServerOnlyFiltering = false,
    };

    public Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default)
        => Task.FromResult<ModCatalogItem?>(null);

    public Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ModCatalogItem>>(new List<ModCatalogItem>());

    public Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ModCatalogItem>>(new List<ModCatalogItem>());
}
