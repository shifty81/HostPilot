using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Adapters;

/// <summary>
/// Scaffold handler for mod installation operations.
/// Connects to the mod provider pipeline once the full install flow is wired.
/// </summary>
public sealed class InstallModsOperationHandler : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.InstallMods;

    public Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        // Scaffold: mod install pipeline integration pending.
        // The full flow will resolve mod IDs from the payload, download via the mod provider
        // registry, and copy into the server's mod folder.
        return Task.FromResult(OperationResult.Success($"Mod install queued for '{context.TargetId}' (scaffold)."));
    }
}
