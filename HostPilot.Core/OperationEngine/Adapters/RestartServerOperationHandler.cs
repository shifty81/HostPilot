using HostPilot.Core.Models;
using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.OperationEngine.Services;
using HostPilot.Core.Services;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class RestartServerOperationHandler(
    ServerManager serverManager,
    IEnumerable<ServerConfig> serverConfigs,
    ServerStateService stateService) : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.RestartServer;

    public Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        var config = serverConfigs.FirstOrDefault(x => string.Equals(x.Name, context.TargetId, StringComparison.OrdinalIgnoreCase));
        if (config is null)
            return Task.FromResult(OperationResult.Failure($"Server '{context.TargetId}' was not found."));

        try
        {
            stateService.SetStatus(config.Name, ServerRuntimeStatus.Stopping);
            serverManager.RestartServer(config.Name);
            stateService.Capture(config);
            return Task.FromResult(OperationResult.Success($"Restarted '{config.Name}'."));
        }
        catch (Exception ex)
        {
            stateService.SetStatus(config.Name, ServerRuntimeStatus.Crashed, ex.Message);
            return Task.FromResult(OperationResult.Failure($"Failed to restart '{config.Name}'.", ex));
        }
    }
}
