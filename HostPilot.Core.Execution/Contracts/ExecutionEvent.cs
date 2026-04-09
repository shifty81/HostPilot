namespace HostPilot.Core.Execution.Contracts;

public sealed record ExecutionEvent(
    string EventType,
    string PlanId,
    string WorkItemId,
    string? NodeId,
    string Message,
    DateTimeOffset TimestampUtc);
