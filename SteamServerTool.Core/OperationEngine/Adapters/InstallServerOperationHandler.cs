using SteamServerTool.Core.Models;
using SteamServerTool.Core.OperationEngine.Abstractions;
using SteamServerTool.Core.OperationEngine.Models;
using SteamServerTool.Core.OperationEngine.Services;
using SteamServerTool.Core.Services;

namespace SteamServerTool.Core.OperationEngine.Adapters;

public sealed class InstallServerOperationHandler(
    SteamCmdService steamCmdService,
    IEnumerable<ServerConfig> serverConfigs,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => SteamServerTool.Core.OperationEngine.Models.OperationType.InstallServer;

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
