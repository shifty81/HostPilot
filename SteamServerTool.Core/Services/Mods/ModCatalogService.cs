using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public sealed class ModCatalogService
{
    private readonly ModProviderRegistry _providerRegistry;

    public ModCatalogService(ModProviderRegistry providerRegistry)
    {
        _providerRegistry = providerRegistry;
    }

    public async Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModSupportProfile profile, ModBrowserQuery query, CancellationToken cancellationToken = default)
    {
        var providers = _providerRegistry.GetProviders(profile.EnabledProviders.Count == 0
            ? new[] { profile.PrimaryProvider }
            : profile.EnabledProviders);

        var results = new List<ModCatalogItem>();
        foreach (var provider in providers)
        {
            var providerResults = await provider.SearchAsync(query, cancellationToken).ConfigureAwait(false);
            results.AddRange(providerResults);
        }

        return results
            .OrderByDescending(x => x.UpdatedUtc)
            .ThenBy(x => x.Name)
            .ToList();
    }
}
