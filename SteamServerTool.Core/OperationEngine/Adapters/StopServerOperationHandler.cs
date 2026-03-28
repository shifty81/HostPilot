using SteamServerTool.Core.Models;
using SteamServerTool.Core.OperationEngine.Abstractions;
using SteamServerTool.Core.OperationEngine.Models;
using SteamServerTool.Core.OperationEngine.Services;
using SteamServerTool.Core.Services;

namespace SteamServerTool.Core.OperationEngine.Adapters;

public sealed class StopServerOperationHandler(
    ServerManager serverManager,
    IEnumerable<ServerConfig> serverConfigs,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => SteamServerTool.Core.OperationEngine.Models.OperationType.StopServer;

    public Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        var config = serverConfigs.FirstOrDefault(x => string.Equals(x.Name, context.TargetId, StringComparison.OrdinalIgnoreCase));
        if (config is null)
            return Task.FromResult(OperationResult.Failure($"Server '{context.TargetId}' was not found."));

        try
        {
            stateService.SetStatus(config.Name, ServerRuntimeStatus.Stopping);
            serverManager.StopServer(config.Name);
            stateService.Capture(config);
            return Task.FromResult(OperationResult.Success($"Stopped '{config.Name}'."));
        }
        catch (Exception ex)
        {
            stateService.SetStatus(config.Name, ServerRuntimeStatus.Crashed, ex.Message);
            return Task.FromResult(OperationResult.Failure($"Failed to stop '{config.Name}'.", ex));
        }
    }
}
