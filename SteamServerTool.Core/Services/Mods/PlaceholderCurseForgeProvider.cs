using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public sealed class PlaceholderCurseForgeProvider : IModProvider
{
    public ModProviderType ProviderType => ModProviderType.CurseForge;

    public ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = true,
        SupportsVersionFiltering = true,
        SupportsCategories = true,
        SupportsServerOnlyFiltering = true,
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
                Id = "example-jei",
                Name = "Example JEI",
                Summary = "Placeholder scaffold result for Minecraft-like provider integration.",
                ProviderType = ProviderType,
                UpdatedUtc = DateTimeOffset.UtcNow,
                SupportsServerSideInstall = true,
            }
        };

        return Task.FromResult(items);
    }
}
