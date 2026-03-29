using HostPilot.Core.Models;
using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.OperationEngine.Services;
using HostPilot.Core.Services;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class BackupServerOperationHandler(
    BackupService backupService,
    IEnumerable<ServerConfig> serverConfigs,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.BackupServer;

    public Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        var config = serverConfigs.FirstOrDefault(x => string.Equals(x.Name, context.TargetId, StringComparison.OrdinalIgnoreCase));
        if (config is null)
            return Task.FromResult(OperationResult.Failure($"Server '{context.TargetId}' was not found."));

        try
        {
            var backupPath = backupService.CreateBackup(config);
            stateService.Capture(config);
            return Task.FromResult(OperationResult.Success($"Backup created for '{config.Name}'.", data: backupPath));
        }
        catch (Exception ex)
        {
            return Task.FromResult(OperationResult.Failure($"Failed to back up '{config.Name}'.", ex));
        }
    }
}
