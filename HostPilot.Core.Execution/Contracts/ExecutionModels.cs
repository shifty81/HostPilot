namespace HostPilot.Core.Execution.Contracts;

public enum ExecutionPlanState
{
    Draft,
    Ready,
    Running,
    Completed,
    Failed,
    Cancelled,
    PartiallyCompleted
}

public enum WorkItemState
{
    Pending,
    Leased,
    Dispatching,
    Running,
    Succeeded,
    Failed,
    Cancelled,
    Skipped,
    TimedOut
}

public sealed record PlacementConstraint(
    string Key,
    string Operator,
    string Value);

public sealed record ResourceRequirements(
    int CpuCores,
    int MemoryMb,
    int StorageMb,
    bool RequiresSteamCmd,
    bool RequiresRcon,
    IReadOnlyList<string> RequiredCapabilities);

public sealed record ExecutionWorkItem(
    string WorkItemId,
    string OperationType,
    string ProviderId,
    string TargetServerId,
    ResourceRequirements Resources,
    IReadOnlyList<PlacementConstraint> PlacementConstraints,
    IReadOnlyList<string> DependsOnWorkItemIds,
    int MaxAttempts,
    TimeSpan Timeout,
    IDictionary<string, string> Arguments);

public sealed record ExecutionPlan(
    string PlanId,
    string Name,
    string RequestedBy,
    DateTimeOffset CreatedUtc,
    IReadOnlyList<ExecutionWorkItem> WorkItems);

public sealed record ExecutionLease(
    string LeaseId,
    string PlanId,
    string WorkItemId,
    string NodeId,
    DateTimeOffset AcquiredUtc,
    DateTimeOffset ExpiresUtc,
    int Attempt);

public sealed record DispatchDecision(
    string NodeId,
    string WorkItemId,
    string Reason,
    int PriorityScore);

public sealed record WorkItemRuntimeState(
    string PlanId,
    string WorkItemId,
    WorkItemState State,
    int Attempt,
    string? NodeId,
    string? Message,
    DateTimeOffset UpdatedUtc);
