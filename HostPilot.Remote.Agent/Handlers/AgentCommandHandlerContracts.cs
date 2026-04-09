namespace HostPilot.Remote.Agent.Handlers;

public interface IAgentCommandHandler
{
    string CommandType { get; }
    Task<AgentCommandResult> HandleAsync(AgentCommandContext context, CancellationToken cancellationToken);
}

public sealed class AgentCommandContext
{
    public required string CorrelationId { get; init; }
    public required string NodeId { get; init; }
    public required string CommandType { get; init; }
    public required string PayloadJson { get; init; }
    public required Action<AgentCommandProgress> ReportProgress { get; init; }
    public required Func<string, string, Task> WriteAuditAsync { get; init; }
}

public sealed class AgentCommandProgress
{
    public int Percent { get; init; }
    public string Stage { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;
}

public sealed class AgentCommandResult
{
    public bool Succeeded { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? OutputJson { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    public static AgentCommandResult Success(string status, string message, string? outputJson = null) =>
        new() { Succeeded = true, Status = status, Message = message, OutputJson = outputJson };

    public static AgentCommandResult Failure(string status, string message) =>
        new() { Succeeded = false, Status = status, Message = message };
}
