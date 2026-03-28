using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public interface IModOperationQueue
{
    Task QueueInstallAsync(ModInstallRequest request, CancellationToken cancellationToken = default);
    Task QueueLocalInstallAsync(string serverId, string importId, CancellationToken cancellationToken = default);
    Task QueueRemoveAsync(string serverId, string installedModId, CancellationToken cancellationToken = default);
}
