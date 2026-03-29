using HostPilot.Core.Models;
using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.OperationEngine.Services;
using HostPilot.Core.Services;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class UpdateServerOperationHandler(
    SteamCmdService steamCmdService,
    IEnumerable<ServerConfig> serverConfigs,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.UpdateServer;

    public async Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        var config = serverConfigs.FirstOrDefault(x => string.Equals(x.Name, context.TargetId, StringComparison.OrdinalIgnoreCase));
        if (config is null)
            return OperationResult.Failure($"Server '{context.TargetId}' was not found.");

        stateService.SetStatus(config.Name, ServerRuntimeStatus.Installing);
        var ok = await steamCmdService.InstallOrUpdateServer(config);
        if (!ok)
        {
            stateService.SetStatus(config.Name, ServerRuntimeStatus.FailedInstall, "SteamCMD update failed.");
            return OperationResult.Failure($"SteamCMD failed to update '{config.Name}'.", retryable: true);
        }

        stateService.Capture(config);
        return OperationResult.Success($"Updated '{config.Name}'.");
    }
}
