using System.Net.Http.Headers;
using System.Text.Json;
using HostPilot.Core.Models.Mods;
using HostPilot.Core.Models.Mods.Http;

namespace HostPilot.Core.Services.Mods;

public sealed class CurseForgeHttpProvider : IModProvider
{
    private readonly HttpClient _httpClient;
    private readonly CurseForgeApiOptions _options;

    public CurseForgeHttpProvider(HttpClient httpClient, CurseForgeApiOptions options)
    {
        _httpClient = httpClient;
        _options = options;

        if (_httpClient.BaseAddress is null)
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);

        if (!_httpClient.DefaultRequestHeaders.Accept.Any())
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(_options.ApiKey) && !_httpClient.DefaultRequestHeaders.Contains("x-api-key"))
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
    }

    public ModProviderType ProviderType => ModProviderType.CurseForge;

    public ModProviderCapabilities GetCapabilities() => new()
    {
        SupportsSearch = true,
        SupportsDependencies = true,
        SupportsVersionFiltering = true,
        SupportsCategories = true,
        SupportsServerOnlyFiltering = true,
    };

    public async Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default)
    {
        var uri = BuildSearchUri(query);
        using var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<ModCatalogItem>();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!document.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            return Array.Empty<ModCatalogItem>();

        var items = new List<ModCatalogItem>();
        foreach (var entry in data.EnumerateArray())
            items.Add(ParseMod(entry));
        return items;
    }

    public async Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"/v1/mods/{Uri.EscapeDataString(modId)}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!document.RootElement.TryGetProperty("data", out var data))
            return null;

        return ParseMod(data);
    }

    public async Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default)
    {
        var fileId = TryReadPrimaryFileId(item);
        if (fileId is null)
            return Array.Empty<ModCatalogItem>();

        using var response = await _httpClient.GetAsync($"/v1/mods/{Uri.EscapeDataString(item.Id)}/files/{fileId}", cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<ModCatalogItem>();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!document.RootElement.TryGetProperty("data", out var data))
            return Array.Empty<ModCatalogItem>();
        if (!data.TryGetProperty("dependencies", out var dependencies) || dependencies.ValueKind != JsonValueKind.Array)
            return Array.Empty<ModCatalogItem>();

        var results = new List<ModCatalogItem>();
        foreach (var dep in dependencies.EnumerateArray())
        {
            if (!dep.TryGetProperty("modId", out var modIdEl))
                continue;

            var dependencyId = modIdEl.GetInt32().ToString();
            results.Add(new ModCatalogItem
            {
                Id = dependencyId,
                Name = dependencyId,
                ProviderType = ProviderType,
                SupportsServerSideInstall = true,
            });
        }

        return results;
    }

    private string BuildSearchUri(ModBrowserQuery query)
    {
        var values = new List<string>
        {
            $"gameId={_options.GameId}",
            $"pageSize={Math.Min(Math.Max(query.PageSize, 1), _options.PageSizeLimit)}",
            $"index={Math.Max(0, (query.Page - 1) * query.PageSize)}",
            "sortField=2"
        };

        if (!string.IsNullOrWhiteSpace(query.SearchText))
            values.Add($"searchFilter={Uri.EscapeDataString(query.SearchText)}");

        if (!string.IsNullOrWhiteSpace(query.GameVersion))
            values.Add($"gameVersion={Uri.EscapeDataString(query.GameVersion)}");

        var loaderType = CurseForgeModLoaderMapper.TryMap(query.Loader ?? query.LoaderType);
        if (loaderType.HasValue)
            values.Add($"modLoaderType={loaderType.Value}");

        return "/v1/mods/search?" + string.Join("&", values);
    }

    private static ModCatalogItem ParseMod(JsonElement item)
    {
        var id = item.TryGetProperty("id", out var idElement) ? idElement.GetInt32().ToString() : Guid.NewGuid().ToString("N");
        var name = item.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : id;
        var summary = item.TryGetProperty("summary", out var summaryElement) ? summaryElement.GetString() : null;
        var updatedUtc = item.TryGetProperty("dateModified", out var modifiedElement) && modifiedElement.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(modifiedElement.GetString(), out var parsedModified)
            ? parsedModified
            : (DateTimeOffset?)null;
        var authors = ReadAuthors(item);
        var thumbnail = ReadLogo(item);
        var latestFile = TryReadLatestFile(item);

        return new ModCatalogItem
        {
            Id = id,
            Name = name ?? id,
            Summary = summary,
            ProviderType = ModProviderType.CurseForge,
            Version = latestFile?.Filename,
            Author = string.Join(", ", authors),
            ThumbnailUrl = thumbnail,
            DownloadUrl = latestFile?.DownloadUrl,
            UpdatedUtc = updatedUtc,
            HasDependencies = latestFile?.HasDependencies ?? false,
            SupportsServerSideInstall = true,
            DependencyIds = latestFile?.DependencyIds ?? new List<string>(),
            Tags = latestFile?.PrimaryCategoryName is null ? new List<string>() : new List<string> { latestFile.PrimaryCategoryName, $"file:{latestFile.FileId}" },
        };
    }

    private static int? TryReadPrimaryFileId(ModCatalogItem item)
    {
        foreach (var tag in item.Tags)
        {
            if (!tag.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                continue;

            if (int.TryParse(tag[5..], out var fileId))
                return fileId;
        }

        return null;
    }

    private static string[] ReadAuthors(JsonElement item)
    {
        if (!item.TryGetProperty("authors", out var authorsElement) || authorsElement.ValueKind != JsonValueKind.Array)
            return Array.Empty<string>();

        return authorsElement.EnumerateArray()
            .Select(a => a.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null)
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Cast<string>()
            .ToArray();
    }

    private static string? ReadLogo(JsonElement item)
    {
        if (!item.TryGetProperty("logo", out var logo) || logo.ValueKind != JsonValueKind.Object)
            return null;

        return logo.TryGetProperty("thumbnailUrl", out var thumb) ? thumb.GetString() : null;
    }

    private static CurseForgeLatestFile? TryReadLatestFile(JsonElement item)
    {
        if (!item.TryGetProperty("latestFiles", out var latestFiles) || latestFiles.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var file in latestFiles.EnumerateArray())
        {
            var fileId = file.TryGetProperty("id", out var idEl) ? idEl.GetInt32() : 0;
            var fileName = file.TryGetProperty("fileName", out var nameEl) ? nameEl.GetString() : null;
            var downloadUrl = file.TryGetProperty("downloadUrl", out var urlEl) ? urlEl.GetString() : null;
            var dependencies = new List<string>();
            var hasDependencies = false;

            if (file.TryGetProperty("dependencies", out var depEl) && depEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var dep in depEl.EnumerateArray())
                {
                    if (!dep.TryGetProperty("modId", out var modIdEl))
                        continue;
                    hasDependencies = true;
                    dependencies.Add(modIdEl.GetInt32().ToString());
                }
            }

            string? primaryCategoryName = null;
            if (file.TryGetProperty("sortableGameVersions", out var versionsEl) && versionsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var v in versionsEl.EnumerateArray())
                {
                    if (v.TryGetProperty("gameVersionName", out var gvName) && !string.IsNullOrWhiteSpace(gvName.GetString()))
                    {
                        primaryCategoryName = gvName.GetString();
                        break;
                    }
                }
            }

            return new CurseForgeLatestFile(fileId, fileName, downloadUrl, hasDependencies, dependencies, primaryCategoryName);
        }

        return null;
    }

    private sealed record CurseForgeLatestFile(int FileId, string? Filename, string? DownloadUrl, bool HasDependencies, List<string> DependencyIds, string? PrimaryCategoryName);
}
