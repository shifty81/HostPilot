namespace HostPilot.Remote.Updates.Abstractions;

using HostPilot.Remote.Contracts.Models;

public interface IRemoteUpdateCoordinator
{
    Task RunPlanAsync(RemoteUpdatePlan plan, CancellationToken cancellationToken = default);
}
