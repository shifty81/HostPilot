using System.Text;
using System.Text.Json;
using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

/// <summary>
/// Official Steam Web API support is intentionally limited here to item lookup by ID/URL.
/// Steam's public item-details endpoint is open, but subscription and user-library enumeration
/// require publisher-authenticated endpoints and should not be called directly from a desktop client.
/// </summary>
public sealed class SteamWorkshopApiProvider : IModProvider
{
    private const string PublishedFileDetailsEndpoint = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";
    private readonly HttpClient _httpClient;

    public SteamWorkshopApiProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ModProviderType ProviderType => ModProviderType.SteamWorkshop;

    public ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = false,
        SupportsVersionFiltering = false,
        SupportsCategories = false,
        SupportsServerOnlyFiltering = false,
    };

    public async Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default)
    {
        var publishedFileId = SteamWorkshopUrlParser.ExtractPublishedFileId(query.SearchText);
        if (string.IsNullOrWhiteSpace(publishedFileId))
            return Array.Empty<ModCatalogItem>();

        var item = await GetByIdAsync(publishedFileId, cancellationToken).ConfigureAwait(false);
        return item is null ? Array.Empty<ModCatalogItem>() : new[] { item };
    }

    public async Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default)
    {
        var normalizedId = SteamWorkshopUrlParser.ExtractPublishedFileId(modId) ?? modId;

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["itemcount"] = "1",
            ["publishedfileids[0]"] = normalizedId,
        });

        using var response = await _httpClient.PostAsync(PublishedFileDetailsEndpoint, content, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!document.RootElement.TryGetProperty("response", out var responseElement))
            return null;
        if (!responseElement.TryGetProperty("publishedfiledetails", out var details) || details.ValueKind != JsonValueKind.Array || details.GetArrayLength() == 0)
            return null;

        var item = details[0];
        var title = item.TryGetProperty("title", out var titleEl) ? titleEl.GetString() : normalizedId;
        var description = item.TryGetProperty("description", out var descEl) ? descEl.GetString() : null;
        var previewUrl = item.TryGetProperty("preview_url", out var previewEl) ? previewEl.GetString() : null;
        var fileSize = item.TryGetProperty("file_size", out var sizeEl) && sizeEl.TryGetInt64(out var parsedSize) ? parsedSize : (long?)null;
        var timeUpdated = item.TryGetProperty("time_updated", out var timeEl) && timeEl.TryGetInt64(out var ts)
            ? DateTimeOffset.FromUnixTimeSeconds(ts)
            : (DateTimeOffset?)null;
        var consumerAppId = item.TryGetProperty("consumer_app_id", out var appEl) ? appEl.GetInt32().ToString() : null;

        return new ModCatalogItem
        {
            Id = normalizedId,
            Name = title ?? normalizedId,
            Summary = description,
            ThumbnailUrl = previewUrl,
            DownloadUrl = $"https://steamcommunity.com/sharedfiles/filedetails/?id={normalizedId}",
            SizeBytes = fileSize,
            UpdatedUtc = timeUpdated,
            ProviderType = ProviderType,
            SupportsServerSideInstall = true,
            Tags = consumerAppId is null ? new List<string>() : new List<string> { $"app:{consumerAppId}" },
        };
    }

    public Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ModCatalogItem>>(Array.Empty<ModCatalogItem>());
}
