using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.Services;
using HostPilot.Core.Services.SteamCmd;

namespace HostPilot.Core.OperationEngine.Adapters;

public sealed class ValidateServerOperationHandler(SteamCmdRunner steamCmd) : IOperationHandler
{
    public string OperationType => HostPilot.Core.OperationEngine.Models.OperationType.ValidateServer;

    public async Task<OperationResult> ExecuteAsync(
        OperationContext context,
        IReadOnlyDictionary<string, object?> payload,
        CancellationToken cancellationToken)
    {
        var installDir = payload.TryGetValue("installDir", out var dir) ? dir?.ToString() : null;
        if (string.IsNullOrWhiteSpace(installDir) || !Directory.Exists(installDir))
        {
            return OperationResult.Failure($"Install directory not found: '{installDir}'.");
        }

        var appId = payload.TryGetValue("appId", out var id) ? id?.ToString() : null;
        if (string.IsNullOrWhiteSpace(appId))
        {
            return OperationResult.Failure("No appId provided for validation.");
        }

        var profile = new SteamCmdProfile
        {
            ProfileName = context.TargetId,
            InstallDirectory = installDir,
            AppId = appId
        };

        var logs = new List<string>();
        var result = await steamCmd.RunAsync(
            profile,
            SteamCmdJobKind.Validate,
            entry => logs.Add(entry.Message),
            (_, _) => { },
            cancellationToken);

        return result.Succeeded
            ? OperationResult.Success($"Validation complete for '{context.TargetId}'.")
            : OperationResult.Failure($"Validation failed for '{context.TargetId}': {result.Summary}");
    }
}
