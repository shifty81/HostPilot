using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public interface IModInstallCoordinator
{
    Task<IReadOnlyList<InstalledModEntry>> GetInstalledAsync(ServerConfig serverConfig, ModSupportProfile profile, CancellationToken cancellationToken = default);
    Task<InstalledModEntry> InstallCatalogItemAsync(ServerConfig serverConfig, ModSupportProfile profile, ModCatalogItem item, CancellationToken cancellationToken = default);
}
