using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public sealed class PlaceholderVintageStoryProvider : IModProvider
{
    public ModProviderType ProviderType => ModProviderType.VintageStory;

    public ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = true,
        SupportsVersionFiltering = true,
        SupportsCategories = true,
        SupportsServerOnlyFiltering = false,
    };

    public Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default)
        => Task.FromResult<ModCatalogItem?>(new ModCatalogItem
        {
            Id = modId,
            Name = modId,
            ProviderType = ProviderType,
            UpdatedUtc = DateTimeOffset.UtcNow,
        });

    public Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ModCatalogItem>>(new List<ModCatalogItem>());

    public Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ModCatalogItem> items = new List<ModCatalogItem>
        {
            new()
            {
                Id = "vs-example",
                Name = "Vintage Story Example Mod",
                Summary = "Placeholder scaffold result for Vintage Story browser integration.",
                ProviderType = ProviderType,
                UpdatedUtc = DateTimeOffset.UtcNow,
            }
        };

        return Task.FromResult(items);
    }
}
