using System.Collections.Generic;

namespace SteamServerTool.Core.Models.Mods.Http;

public sealed class ModProviderEndpointOptions
{
    public string ProviderName { get; set; } = string.Empty;
    public string? SearchEndpointTemplate { get; set; }
    public string? ItemByIdEndpointTemplate { get; set; }
    public string? DependencyEndpointTemplate { get; set; }
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    public string? SearchTextQueryKey { get; set; } = "q";
    public string? GameVersionQueryKey { get; set; }
    public string? LoaderQueryKey { get; set; }
}
