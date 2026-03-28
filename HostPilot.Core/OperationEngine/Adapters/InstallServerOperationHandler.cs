using HostPilot.Core.Models;
using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.OperationEngine.Services;
using HostPilot.Core.Services;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class InstallServerOperationHandler(
    SteamCmdService steamCmdService,
    IEnumerable<ServerConfig> serverConfigs,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.InstallServer;

    public async Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        var config = serverConfigs.FirstOrDefault(x => string.Equals(x.Name, context.TargetId, StringComparison.OrdinalIgnoreCase));
        if (config is null)
            return OperationResult.Failure($"Server '{context.TargetId}' was not found.");

        stateService.SetStatus(config.Name, ServerRuntimeStatus.Installing);
        var ok = await steamCmdService.InstallOrUpdateServer(config);
        if (!ok)
        {
            stateService.SetStatus(config.Name, ServerRuntimeStatus.FailedInstall, "SteamCMD install/update failed.");
            return OperationResult.Failure($"SteamCMD failed to install or update '{config.Name}'.", retryable: true);
        }

        stateService.Capture(config);
        return OperationResult.Success($"Installed or updated '{config.Name}'.");
    }
}
