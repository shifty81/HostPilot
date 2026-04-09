using System.Text.Json;

namespace HostPilot.Remote.Agent.Handlers;

public sealed class StartServerHandler : IAgentCommandHandler
{
    public string CommandType => "server.start";

    public async Task<AgentCommandResult> HandleAsync(AgentCommandContext context, CancellationToken cancellationToken)
    {
        var request = JsonSerializer.Deserialize<StartServerRequest>(context.PayloadJson) ?? new StartServerRequest();

        context.ReportProgress(new AgentCommandProgress { Percent = 10, Stage = "validate", Message = $"Validating deployment '{request.DeploymentId}'." });
        await context.WriteAuditAsync("server.start", $"Start requested for deployment '{request.DeploymentId}'.");

        context.ReportProgress(new AgentCommandProgress { Percent = 55, Stage = "launch", Message = "Launching process runner scaffold." });
        await Task.Delay(25, cancellationToken);

        context.ReportProgress(new AgentCommandProgress { Percent = 100, Stage = "complete", Message = "Server start handler completed." });
        return AgentCommandResult.Success("started", "Server start scaffold completed.");
    }
}

public sealed class StartServerRequest
{
    public string DeploymentId { get; init; } = string.Empty;
    public string InstanceId { get; init; } = string.Empty;
}
