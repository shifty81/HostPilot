namespace SteamServerTool.Core.OperationEngine.Models;

public sealed class OperationRequest
{
    public string OperationId { get; init; } = Guid.NewGuid().ToString("N");
    public string Type { get; init; } = string.Empty;
    public string TargetId { get; init; } = string.Empty;
    public OperationPriority Priority { get; init; } = OperationPriority.Normal;
    public int MaxRetries { get; init; } = 0;
    public TimeSpan? Timeout { get; init; }
    public IReadOnlyDictionary<string, object?> Payload { get; init; } = new Dictionary<string, object?>();
    public IReadOnlyCollection<string> DependsOnOperationIds { get; init; } = Array.Empty<string>();
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
