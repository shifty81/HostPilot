using System.Text.Json;
using HostPilot.Core.Models.Mods;
using HostPilot.Core.Models.Mods.Http;

namespace HostPilot.Core.Services.Mods;

public sealed class VintageStoryHttpProvider : IModProvider
{
    private readonly HttpClient _httpClient;
    private readonly ModProviderEndpointOptions _options;

    public VintageStoryHttpProvider(HttpClient httpClient, ModProviderEndpointOptions options)
    {
        _httpClient = httpClient;
        _options = options;
        if (_httpClient.BaseAddress is null)
            _httpClient.BaseAddress = new Uri("https://mods.vintagestory.at/");
    }

    public ModProviderType ProviderType => ModProviderType.VintageStory;

    public ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = false,
        SupportsVersionFiltering = true,
        SupportsCategories = true,
        SupportsServerOnlyFiltering = false,
    };

    public async Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default)
    {
        var uri = BuildSearchUri(query);
        using var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<ModCatalogItem>();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        var root = document.RootElement;
        if (root.TryGetProperty("mods", out var modsElement) && modsElement.ValueKind == JsonValueKind.Array)
            return modsElement.EnumerateArray().Select(ParseSummary).ToList();

        if (root.ValueKind == JsonValueKind.Array)
            return root.EnumerateArray().Select(ParseSummary).ToList();

        return Array.Empty<ModCatalogItem>();
    }

    public async Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"api/mod/{Uri.EscapeDataString(modId)}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!document.RootElement.TryGetProperty("mod", out var modElement))
            return null;

        return ParseDetail(modElement);
    }

    public Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ModCatalogItem>>(Array.Empty<ModCatalogItem>());

    private string BuildSearchUri(ModBrowserQuery query)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(query.SearchText))
            parts.Add($"text={Uri.EscapeDataString(query.SearchText)}");
        if (!string.IsNullOrWhiteSpace(query.GameVersion))
            parts.Add($"gv={Uri.EscapeDataString(query.GameVersion)}");
        if (!string.IsNullOrWhiteSpace(query.Category))
            parts.Add($"tagids[]={Uri.EscapeDataString(query.Category)}");
        parts.Add("orderby=downloads");
        parts.Add("orderdirection=desc");
        return "api/mods" + (parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts));
    }

    private static ModCatalogItem ParseSummary(JsonElement item)
    {
        var identifier = ReadIdentifier(item);
        var title = item.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : identifier;
        var summary = item.TryGetProperty("text", out var textEl) ? textEl.GetString() : null;
        var author = item.TryGetProperty("author", out var authorEl) ? authorEl.GetString() : null;
        var updatedUtc = TryReadDate(item, "lastmodified") ?? TryReadDate(item, "lastreleased") ?? TryReadDate(item, "created");
        var logo = item.TryGetProperty("logofile", out var logoEl) ? logoEl.GetString() : null;
        var tags = item.TryGetProperty("tags", out var tagsEl) && tagsEl.ValueKind == JsonValueKind.Array
            ? tagsEl.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!).ToList()
            : new List<string>();

        return new ModCatalogItem
        {
            Id = identifier,
            Name = title ?? identifier,
            Summary = summary,
            Author = author,
            ThumbnailUrl = logo,
            UpdatedUtc = updatedUtc,
            ProviderType = ModProviderType.VintageStory,
            SupportsServerSideInstall = true,
            Tags = tags,
        };
    }

    private static ModCatalogItem ParseDetail(JsonElement item)
    {
        var summary = ParseSummary(item);
        if (item.TryGetProperty("releases", out var releasesEl) && releasesEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var release in releasesEl.EnumerateArray())
            {
                summary.Version = release.TryGetProperty("modversion", out var versionEl) ? versionEl.GetString() : summary.Version;
                summary.DownloadUrl = release.TryGetProperty("mainfile", out var fileEl) ? fileEl.GetString() : summary.DownloadUrl;
                if (release.TryGetProperty("tags", out var releaseTags) && releaseTags.ValueKind == JsonValueKind.Array)
                {
                    foreach (var tag in releaseTags.EnumerateArray())
                    {
                        var value = tag.GetString();
                        if (!string.IsNullOrWhiteSpace(value) && !summary.Tags.Contains(value))
                            summary.Tags.Add(value);
                    }
                }
                break;
            }
        }

        return summary;
    }

    private static string ReadIdentifier(JsonElement item)
    {
        if (item.TryGetProperty("modidstr", out var modidStr) && !string.IsNullOrWhiteSpace(modidStr.GetString()))
            return modidStr.GetString()!;
        if (item.TryGetProperty("urlalias", out var alias) && !string.IsNullOrWhiteSpace(alias.GetString()))
            return alias.GetString()!;
        if (item.TryGetProperty("modid", out var modid))
            return modid.ToString();
        return Guid.NewGuid().ToString("N");
    }

    private static DateTimeOffset? TryReadDate(JsonElement item, string propertyName)
    {
        if (!item.TryGetProperty(propertyName, out var prop) || prop.ValueKind != JsonValueKind.String)
            return null;
        return DateTimeOffset.TryParse(prop.GetString(), out var parsed) ? parsed : null;
    }
}
