using HostPilot.Core.Execution.Contracts;

namespace HostPilot.Core.Execution.Services;

public sealed class RoundRobinNodeSelector : INodeSelector
{
    private int _index;

    public Task<DispatchDecision?> SelectNodeAsync(
        ExecutionWorkItem workItem,
        IReadOnlyList<NodeDescriptor> candidateNodes,
        CancellationToken cancellationToken)
    {
        var eligible = candidateNodes
            .Where(x => x.IsOnline)
            .Where(x => x.AvailableCpuCores >= workItem.Resources.CpuCores)
            .Where(x => x.AvailableMemoryMb >= workItem.Resources.MemoryMb)
            .Where(x => x.AvailableStorageMb >= workItem.Resources.StorageMb)
            .Where(x => !workItem.Resources.RequiresSteamCmd || x.HasSteamCmd)
            .Where(x => !workItem.Resources.RequiresRcon || x.SupportsRcon)
            .OrderByDescending(x => x.HealthScore)
            .ThenBy(x => x.RunningWorkItemCount)
            .ToList();

        if (eligible.Count == 0)
            return Task.FromResult<DispatchDecision?>(null);

        var selected = eligible[_index % eligible.Count];
        _index++;

        return Task.FromResult<DispatchDecision?>(new DispatchDecision(
            selected.NodeId,
            workItem.WorkItemId,
            "Selected by round-robin weighted by health then workload.",
            (int)(selected.HealthScore * 100)));
    }
}
