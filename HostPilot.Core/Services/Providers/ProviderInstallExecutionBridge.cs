using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Providers;

public sealed class ProviderInstallExecutionBridge
{
    private readonly IModOperationDispatcher _dispatcher;

    public ProviderInstallExecutionBridge(IModOperationDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task QueueProviderInstallAsync(
        string serverId,
        string provider,
        string modId,
        IEnumerable<DependencyReviewItem> reviewedDependencies,
        CancellationToken cancellationToken = default)
    {
        var selectedDependencies = reviewedDependencies
            .Where(x => x.IsSelected && !x.IsAlreadyInstalled)
            .Select(x => x.DependencyId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        await _dispatcher.QueueBrowserInstallAsync(serverId, provider, modId, selectedDependencies, cancellationToken).ConfigureAwait(false);
    }

    public Task QueueWorkshopInstallAsync(string serverId, string workshopItemId, CancellationToken cancellationToken = default)
        => _dispatcher.QueueWorkshopInstallAsync(serverId, workshopItemId, cancellationToken);
}
