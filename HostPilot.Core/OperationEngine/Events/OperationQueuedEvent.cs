namespace HostPilot.Core.OperationEngine.Events;

public sealed record OperationQueuedEvent(string OperationId, string Type, string TargetId)
    : OperationEvent(DateTimeOffset.UtcNow);
