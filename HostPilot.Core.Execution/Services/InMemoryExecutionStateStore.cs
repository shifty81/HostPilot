using System.Collections.Concurrent;
using HostPilot.Core.Execution.Contracts;

namespace HostPilot.Core.Execution.Services;

public sealed class InMemoryExecutionStateStore : IExecutionStateStore
{
    private readonly ConcurrentDictionary<string, WorkItemRuntimeState> _states = new();

    public Task SetStateAsync(WorkItemRuntimeState state, CancellationToken cancellationToken)
    {
        _states[$"{state.PlanId}:{state.WorkItemId}"] = state;
        return Task.CompletedTask;
    }

    public Task<WorkItemRuntimeState?> GetStateAsync(string planId, string workItemId, CancellationToken cancellationToken)
    {
        _states.TryGetValue($"{planId}:{workItemId}", out var state);
        return Task.FromResult(state);
    }

    public Task<IReadOnlyList<WorkItemRuntimeState>> GetPlanStatesAsync(string planId, CancellationToken cancellationToken)
    {
        var results = _states.Values.Where(x => x.PlanId == planId).OrderBy(x => x.WorkItemId).ToList();
        return Task.FromResult<IReadOnlyList<WorkItemRuntimeState>>(results);
    }
}
