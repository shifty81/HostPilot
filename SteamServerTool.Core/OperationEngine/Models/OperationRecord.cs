namespace SteamServerTool.Core.OperationEngine.Models;

public sealed class OperationRecord
{
    private readonly List<OperationLogEntry> _logs = new();

    public string OperationId { get; init; } = Guid.NewGuid().ToString("N");
    public string Type { get; init; } = string.Empty;
    public string TargetId { get; init; } = string.Empty;
    public OperationPriority Priority { get; init; } = OperationPriority.Normal;
    public OperationStatus Status { get; private set; } = OperationStatus.Pending;
    public int AttemptCount { get; private set; }
    public int MaxRetries { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public IReadOnlyDictionary<string, object?> Payload { get; init; } = new Dictionary<string, object?>();
    public IReadOnlyCollection<string> DependsOnOperationIds { get; init; } = Array.Empty<string>();
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
    public OperationResult? Result { get; private set; }
    public IReadOnlyList<OperationLogEntry> Logs => _logs;

    public void MarkQueued() => Status = OperationStatus.Queued;

    public void MarkRunning()
    {
        AttemptCount++;
        Status = OperationStatus.Running;
        StartedAtUtc ??= DateTimeOffset.UtcNow;
    }

    public void MarkRetrying(OperationResult result)
    {
        Result = result;
        Status = OperationStatus.Retrying;
    }

    public void MarkCompleted(OperationStatus status, OperationResult result)
    {
        Status = status;
        Result = result;
        CompletedAtUtc = DateTimeOffset.UtcNow;
    }

    public void AddLog(string level, string message) => _logs.Add(new OperationLogEntry(DateTimeOffset.UtcNow, level, message));
}
