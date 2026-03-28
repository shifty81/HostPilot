using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using SteamServerTool.Core.Models.Mods;
using SteamServerTool.Core.Models.Mods.Http;
using SteamServerTool.Core.Services.Mods.Http;

namespace SteamServerTool.Core.Services.Mods;

public sealed class CurseForgeHttpProvider : JsonModProviderBase
{
    public CurseForgeHttpProvider(HttpClient httpClient, ModProviderEndpointOptions options)
        : base(httpClient, options)
    {
    }

    public override ModProviderType ProviderType => ModProviderType.CurseForge;

    public override ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = true,
        SupportsVersionFiltering = true,
        SupportsCategories = true,
        SupportsServerOnlyFiltering = true,
    };

    protected override IReadOnlyList<ModCatalogItem> ParseSearchResults(JsonElement root)
    {
        var results = new List<ModCatalogItem>();
        if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            return results;

        foreach (var item in data.EnumerateArray())
        {
            var id = item.TryGetProperty("id", out var idElement) ? idElement.ToString() : Guid.NewGuid().ToString("N");
            var name = item.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : id;
            var summary = item.TryGetProperty("summary", out var summaryElement) ? summaryElement.GetString() : null;

            results.Add(new ModCatalogItem
            {
                Id = id,
                Name = name ?? id,
                Summary = summary,
                ProviderType = ProviderType,
                Version = TryReadLatestVersion(item),
                UpdatedUtc = DateTimeOffset.UtcNow,
                SupportsServerSideInstall = true,
            });
        }

        return results;
    }

    protected override ModCatalogItem? ParseItem(JsonElement root)
    {
        if (!root.TryGetProperty("data", out var data))
            return null;

        var id = data.TryGetProperty("id", out var idElement) ? idElement.ToString() : string.Empty;
        var name = data.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : id;

        return new ModCatalogItem
        {
            Id = id,
            Name = name ?? id,
            Summary = data.TryGetProperty("summary", out var summaryElement) ? summaryElement.GetString() : null,
            ProviderType = ProviderType,
            Version = TryReadLatestVersion(data),
            UpdatedUtc = DateTimeOffset.UtcNow,
            SupportsServerSideInstall = true,
        };
    }

    private static string? TryReadLatestVersion(JsonElement item)
    {
        if (!item.TryGetProperty("latestFilesIndexes", out var indexes) || indexes.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var idx in indexes.EnumerateArray())
        {
            if (idx.TryGetProperty("filename", out var fileName))
                return fileName.GetString();
        }

        return null;
    }
}
