using HostPilot.Core.Models;
using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.OperationEngine.Services;
using HostPilot.Core.Services;
using HostPilot.Core.Services.Discovery;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class ImportExistingServerOperationHandler(
    ServerManager serverManager,
    ServerImportCoordinator importCoordinator,
    InstalledServerDiscoveryService discoveryService,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.ImportExistingServer;

    public async Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        try
        {
            var scanRoots = context.Metadata.TryGetValue("scanRoot", out var root) && !string.IsNullOrWhiteSpace(root)
                ? new[] { root }
                : null;

            var candidates = await discoveryService.DiscoverAsync(scanRoots, cancellationToken: cancellationToken);
            var targetName = context.TargetId;

            var candidate = candidates.FirstOrDefault(c =>
                string.Equals(c.DisplayName, targetName, StringComparison.OrdinalIgnoreCase));

            if (candidate is null)
                return OperationResult.Failure($"No discovered server matches '{targetName}'.");

            var serverConfig = importCoordinator.BuildImportedConfig(candidate);
            serverManager.Servers.Add(serverConfig);
            stateService.Capture(serverConfig);

            return OperationResult.Success($"Imported existing server '{serverConfig.Name}'.");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to import server '{context.TargetId}'.", ex);
        }
    }
}
