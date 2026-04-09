using System.Text.Json;

namespace HostPilot.Remote.Agent.Handlers;

public sealed class BackupServerHandler : IAgentCommandHandler
{
    public string CommandType => "server.backup";

    public async Task<AgentCommandResult> HandleAsync(AgentCommandContext context, CancellationToken cancellationToken)
    {
        var request = JsonSerializer.Deserialize<BackupServerRequest>(context.PayloadJson) ?? new BackupServerRequest();

        context.ReportProgress(new AgentCommandProgress { Percent = 15, Stage = "prepare", Message = "Preparing backup manifest scaffold." });
        await context.WriteAuditAsync("server.backup", $"Backup requested for '{request.DeploymentId}' to '{request.TargetPath}'.");
        await Task.Delay(25, cancellationToken);

        context.ReportProgress(new AgentCommandProgress { Percent = 100, Stage = "complete", Message = "Backup handler completed." });
        return AgentCommandResult.Success("backup_created", "Backup scaffold completed.");
    }
}

public sealed class BackupServerRequest
{
    public string DeploymentId { get; init; } = string.Empty;
    public string TargetPath { get; init; } = string.Empty;
}
