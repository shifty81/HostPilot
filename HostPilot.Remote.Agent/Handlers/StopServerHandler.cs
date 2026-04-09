using System.Text.Json;

namespace HostPilot.Remote.Agent.Handlers;

public sealed class StopServerHandler : IAgentCommandHandler
{
    public string CommandType => "server.stop";

    public async Task<AgentCommandResult> HandleAsync(AgentCommandContext context, CancellationToken cancellationToken)
    {
        var request = JsonSerializer.Deserialize<StopServerRequest>(context.PayloadJson) ?? new StopServerRequest();

        context.ReportProgress(new AgentCommandProgress { Percent = 25, Stage = "warn", Message = "Issuing graceful shutdown scaffold." });
        await context.WriteAuditAsync("server.stop", $"Stop requested for deployment '{request.DeploymentId}'.");
        await Task.Delay(25, cancellationToken);

        context.ReportProgress(new AgentCommandProgress { Percent = 100, Stage = "complete", Message = "Server stop handler completed." });
        return AgentCommandResult.Success("stopped", "Server stop scaffold completed.");
    }
}

public sealed class StopServerRequest
{
    public string DeploymentId { get; init; } = string.Empty;
    public int GracefulTimeoutSeconds { get; init; } = 30;
}
