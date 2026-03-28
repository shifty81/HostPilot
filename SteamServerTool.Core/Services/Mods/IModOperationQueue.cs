using System.Threading;
using System.Threading.Tasks;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public interface IModOperationQueue
{
    Task QueueInstallAsync(ModInstallRequest request, CancellationToken cancellationToken = default);
    Task QueueLocalInstallAsync(string serverId, string importId, CancellationToken cancellationToken = default);
    Task QueueRemoveAsync(string serverId, string installedModId, CancellationToken cancellationToken = default);
}
