namespace HostPilot.Web.Services;

using System.Collections.Concurrent;
using HostPilot.Web.Models;

public sealed class RolloutCoordinator
{
    private readonly ConcurrentDictionary<string, RolloutPlanDto> _plans = new();
    private readonly ConcurrentDictionary<string, RolloutNodeStateDto> _states = new();

    public Task SavePlanAsync(RolloutPlanDto plan, CancellationToken cancellationToken)
    {
        _plans[plan.RolloutId] = plan;
        foreach (var wave in plan.Waves)
        {
            foreach (var nodeId in wave.NodeIds)
            {
                var key = $"{plan.RolloutId}:{nodeId}";
                _states[key] = new RolloutNodeStateDto
                {
                    RolloutId = plan.RolloutId,
                    NodeId = nodeId,
                    Status = "pending",
                    Percent = 0
                };
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RolloutNodeStateDto>> GetStatesAsync(string rolloutId, CancellationToken cancellationToken)
    {
        var items = _states.Values
            .Where(x => x.RolloutId == rolloutId)
            .OrderBy(x => x.NodeId)
            .ToArray();
        return Task.FromResult<IReadOnlyList<RolloutNodeStateDto>>(items);
    }
}
