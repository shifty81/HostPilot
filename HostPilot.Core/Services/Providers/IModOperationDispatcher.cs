namespace HostPilot.Core.Services.Providers;

public interface IModOperationDispatcher
{
    Task QueueBrowserInstallAsync(string serverId, string provider, string modId, IEnumerable<string> dependencyIds, CancellationToken cancellationToken = default);
    Task QueueWorkshopInstallAsync(string serverId, string workshopItemId, CancellationToken cancellationToken = default);
}
