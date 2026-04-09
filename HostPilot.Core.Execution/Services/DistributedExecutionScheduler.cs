using HostPilot.Core.Execution.Contracts;

namespace HostPilot.Core.Execution.Services;

public sealed class DistributedExecutionScheduler
{
    private readonly INodeCatalog _nodeCatalog;
    private readonly INodeSelector _nodeSelector;
    private readonly ILeaseStore _leaseStore;
    private readonly IExecutionStateStore _stateStore;
    private readonly IRemoteWorkloadExecutor _executor;
    private readonly IExecutionEventSink _events;

    public DistributedExecutionScheduler(
        INodeCatalog nodeCatalog,
        INodeSelector nodeSelector,
        ILeaseStore leaseStore,
        IExecutionStateStore stateStore,
        IRemoteWorkloadExecutor executor,
        IExecutionEventSink events)
    {
        _nodeCatalog = nodeCatalog;
        _nodeSelector = nodeSelector;
        _leaseStore = leaseStore;
        _stateStore = stateStore;
        _executor = executor;
        _events = events;
    }

    public async Task ScheduleAsync(ExecutionPlan plan, CancellationToken cancellationToken)
    {
        var nodes = await _nodeCatalog.GetAvailableNodesAsync(cancellationToken);

        foreach (var item in TopologicalSort(plan.WorkItems))
        {
            if (!await DependenciesSucceededAsync(plan.PlanId, item, cancellationToken))
            {
                await _stateStore.SetStateAsync(new WorkItemRuntimeState(
                    plan.PlanId, item.WorkItemId, WorkItemState.Skipped, 0, null,
                    "Skipped because a dependency did not succeed.",
                    DateTimeOffset.UtcNow), cancellationToken);
                continue;
            }

            var attempt = 1;
            var scheduled = false;

            while (attempt <= item.MaxAttempts && !scheduled)
            {
                var decision = await _nodeSelector.SelectNodeAsync(item, nodes, cancellationToken);
                if (decision is null)
                {
                    await _events.PublishAsync(new ExecutionEvent(
                        "NoNodeAvailable", plan.PlanId, item.WorkItemId, null,
                        "No eligible node was available for dispatch.",
                        DateTimeOffset.UtcNow), cancellationToken);
                    break;
                }

                var lease = await _leaseStore.TryAcquireLeaseAsync(plan, item, decision.NodeId, attempt, cancellationToken);
                if (lease is null)
                {
                    attempt++;
                    continue;
                }

                await _stateStore.SetStateAsync(new WorkItemRuntimeState(
                    plan.PlanId, item.WorkItemId, WorkItemState.Dispatching, attempt, decision.NodeId,
                    decision.Reason, DateTimeOffset.UtcNow), cancellationToken);

                await _events.PublishAsync(new ExecutionEvent(
                    "Dispatching", plan.PlanId, item.WorkItemId, decision.NodeId,
                    decision.Reason, DateTimeOffset.UtcNow), cancellationToken);

                await _executor.DispatchAsync(plan, item, lease, cancellationToken);
                scheduled = true;
            }

            if (!scheduled)
            {
                await _stateStore.SetStateAsync(new WorkItemRuntimeState(
                    plan.PlanId, item.WorkItemId, WorkItemState.Failed, attempt - 1, null,
                    "Could not acquire lease or dispatch the work item.",
                    DateTimeOffset.UtcNow), cancellationToken);
            }
        }
    }

    private async Task<bool> DependenciesSucceededAsync(
        string planId,
        ExecutionWorkItem item,
        CancellationToken cancellationToken)
    {
        foreach (var dependencyId in item.DependsOnWorkItemIds)
        {
            var state = await _stateStore.GetStateAsync(planId, dependencyId, cancellationToken);
            if (state is null || state.State != WorkItemState.Succeeded)
                return false;
        }

        return true;
    }

    private static IReadOnlyList<ExecutionWorkItem> TopologicalSort(IReadOnlyList<ExecutionWorkItem> items)
    {
        var lookup = items.ToDictionary(x => x.WorkItemId);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<ExecutionWorkItem>();

        void Visit(ExecutionWorkItem item)
        {
            if (!visited.Add(item.WorkItemId))
                return;

            foreach (var dependency in item.DependsOnWorkItemIds)
                Visit(lookup[dependency]);

            result.Add(item);
        }

        foreach (var item in items)
            Visit(item);

        return result.DistinctBy(x => x.WorkItemId).ToList();
    }
}
