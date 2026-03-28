using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Models;
using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public interface IModInstallCoordinator
{
    Task<IReadOnlyList<InstalledModEntry>> GetInstalledAsync(ServerConfig serverConfig, ModSupportProfile profile, CancellationToken cancellationToken = default);
    Task<InstalledModEntry> InstallCatalogItemAsync(ServerConfig serverConfig, ModSupportProfile profile, ModCatalogItem item, CancellationToken cancellationToken = default);
}
