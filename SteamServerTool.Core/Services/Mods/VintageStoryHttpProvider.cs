using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using SteamServerTool.Core.Models.Mods;
using SteamServerTool.Core.Models.Mods.Http;
using SteamServerTool.Core.Services.Mods.Http;

namespace SteamServerTool.Core.Services.Mods;

public sealed class VintageStoryHttpProvider : JsonModProviderBase
{
    public VintageStoryHttpProvider(HttpClient httpClient, ModProviderEndpointOptions options)
        : base(httpClient, options)
    {
    }

    public override ModProviderType ProviderType => ModProviderType.VintageStory;

    public override ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = true,
        SupportsVersionFiltering = true,
        SupportsCategories = true,
        SupportsServerOnlyFiltering = false,
    };

    protected override IReadOnlyList<ModCatalogItem> ParseSearchResults(JsonElement root)
    {
        var results = new List<ModCatalogItem>();
        if (root.ValueKind != JsonValueKind.Array)
            return results;

        foreach (var item in root.EnumerateArray())
        {
            var id = item.TryGetProperty("modid", out var idElement) ? idElement.GetString() : Guid.NewGuid().ToString("N");
            var name = item.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : id;
            var summary = item.TryGetProperty("text", out var textElement) ? textElement.GetString() : null;

            results.Add(new ModCatalogItem
            {
                Id = id ?? Guid.NewGuid().ToString("N"),
                Name = name ?? id ?? "Unknown Mod",
                Summary = summary,
                ProviderType = ProviderType,
                UpdatedUtc = DateTimeOffset.UtcNow,
                SupportsServerSideInstall = true,
            });
        }

        return results;
    }
}
