using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Models.Mods;
using HostPilot.Core.Models.Mods.Http;

namespace HostPilot.Core.Services.Mods.Http;

public abstract class JsonModProviderBase : IModProvider
{
    private readonly HttpClient _httpClient;
    private readonly ModProviderEndpointOptions _options;

    protected JsonModProviderBase(HttpClient httpClient, ModProviderEndpointOptions options)
    {
        _httpClient = httpClient;
        _options = options;

        foreach (var header in _options.DefaultHeaders)
        {
            if (!_httpClient.DefaultRequestHeaders.Contains(header.Key))
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public abstract ModProviderType ProviderType { get; }

    public abstract ModProviderCapabilities GetCapabilities();

    public virtual async Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.SearchEndpointTemplate))
            return Array.Empty<ModCatalogItem>();

        var uri = BuildUri(_options.SearchEndpointTemplate!, query);
        using var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<ModCatalogItem>();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        return ParseSearchResults(document.RootElement);
    }

    public virtual async Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ItemByIdEndpointTemplate))
            return null;

        var uri = _options.ItemByIdEndpointTemplate!.Replace("{id}", Uri.EscapeDataString(modId));
        using var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        return ParseItem(document.RootElement);
    }

    public virtual async Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.DependencyEndpointTemplate))
            return Array.Empty<ModCatalogItem>();

        var uri = _options.DependencyEndpointTemplate!.Replace("{id}", Uri.EscapeDataString(item.Id));
        using var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<ModCatalogItem>();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        return ParseDependencyResults(document.RootElement);
    }

    protected virtual IReadOnlyList<ModCatalogItem> ParseSearchResults(JsonElement root) => Array.Empty<ModCatalogItem>();
    protected virtual ModCatalogItem? ParseItem(JsonElement root) => null;
    protected virtual IReadOnlyList<ModCatalogItem> ParseDependencyResults(JsonElement root) => Array.Empty<ModCatalogItem>();

    private string BuildUri(string template, ModBrowserQuery query)
    {
        var uri = template;
        if (!string.IsNullOrWhiteSpace(_options.SearchTextQueryKey))
            uri = uri.Replace("{" + _options.SearchTextQueryKey + "}", Uri.EscapeDataString(query.SearchText ?? string.Empty));
        if (!string.IsNullOrWhiteSpace(_options.GameVersionQueryKey))
            uri = uri.Replace("{" + _options.GameVersionQueryKey + "}", Uri.EscapeDataString(query.GameVersion ?? string.Empty));
        if (!string.IsNullOrWhiteSpace(_options.LoaderQueryKey))
            uri = uri.Replace("{" + _options.LoaderQueryKey + "}", Uri.EscapeDataString(query.LoaderType ?? string.Empty));
        return uri;
    }
}
