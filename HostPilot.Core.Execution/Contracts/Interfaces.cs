namespace HostPilot.Core.Execution.Contracts;

public interface INodeCatalog
{
    Task<IReadOnlyList<NodeDescriptor>> GetAvailableNodesAsync(CancellationToken cancellationToken);
    Task<NodeDescriptor?> GetNodeAsync(string nodeId, CancellationToken cancellationToken);
}

public interface INodeSelector
{
    Task<DispatchDecision?> SelectNodeAsync(
        ExecutionWorkItem workItem,
        IReadOnlyList<NodeDescriptor> candidateNodes,
        CancellationToken cancellationToken);
}

public interface ILeaseStore
{
    Task<ExecutionLease?> TryAcquireLeaseAsync(
        ExecutionPlan plan,
        ExecutionWorkItem workItem,
        string nodeId,
        int attempt,
        CancellationToken cancellationToken);

    Task RenewLeaseAsync(ExecutionLease lease, CancellationToken cancellationToken);
    Task ReleaseLeaseAsync(string leaseId, CancellationToken cancellationToken);
}

public interface IExecutionStateStore
{
    Task SetStateAsync(WorkItemRuntimeState state, CancellationToken cancellationToken);
    Task<WorkItemRuntimeState?> GetStateAsync(string planId, string workItemId, CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkItemRuntimeState>> GetPlanStatesAsync(string planId, CancellationToken cancellationToken);
}

public interface IRemoteWorkloadExecutor
{
    Task DispatchAsync(
        ExecutionPlan plan,
        ExecutionWorkItem workItem,
        ExecutionLease lease,
        CancellationToken cancellationToken);
}

public interface IExecutionEventSink
{
    Task PublishAsync(ExecutionEvent evt, CancellationToken cancellationToken);
}
