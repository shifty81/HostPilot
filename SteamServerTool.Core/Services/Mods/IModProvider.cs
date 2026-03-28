using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public interface IModProvider
{
    ModProviderType ProviderType { get; }
    ModProviderCapabilities GetCapabilities();
    Task<IReadOnlyList<ModCatalogItem>> SearchAsync(ModBrowserQuery query, CancellationToken cancellationToken = default);
    Task<ModCatalogItem?> GetByIdAsync(string modId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModCatalogItem>> ResolveDependenciesAsync(ModCatalogItem item, CancellationToken cancellationToken = default);
}
