using HostPilot.Core.Execution.Contracts;

namespace HostPilot.Core.Execution.Services;

public sealed class ExecutionPlanCompiler
{
    public ExecutionPlan Compile(
        string planId,
        string name,
        string requestedBy,
        IEnumerable<ExecutionWorkItem> items)
    {
        var workItems = items.ToList();
        EnsureUniqueIds(workItems);
        EnsureDependenciesExist(workItems);
        EnsureAcyclic(workItems);

        return new ExecutionPlan(
            planId,
            name,
            requestedBy,
            DateTimeOffset.UtcNow,
            workItems);
    }

    private static void EnsureUniqueIds(IReadOnlyList<ExecutionWorkItem> items)
    {
        var duplicates = items.GroupBy(x => x.WorkItemId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Count > 0)
            throw new InvalidOperationException($"Duplicate work item ids: {string.Join(", ", duplicates)}");
    }

    private static void EnsureDependenciesExist(IReadOnlyList<ExecutionWorkItem> items)
    {
        var ids = items.Select(x => x.WorkItemId).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            foreach (var dependency in item.DependsOnWorkItemIds)
            {
                if (!ids.Contains(dependency))
                    throw new InvalidOperationException($"Work item '{item.WorkItemId}' depends on missing work item '{dependency}'.");
            }
        }
    }

    private static void EnsureAcyclic(IReadOnlyList<ExecutionWorkItem> items)
    {
        var map = items.ToDictionary(x => x.WorkItemId, x => x.DependsOnWorkItemIds);
        var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        bool Dfs(string id)
        {
            if (visited.Contains(id))
                return false;
            if (!visiting.Add(id))
                return true;

            foreach (var dep in map[id])
            {
                if (Dfs(dep))
                    return true;
            }

            visiting.Remove(id);
            visited.Add(id);
            return false;
        }

        foreach (var item in items)
        {
            if (Dfs(item.WorkItemId))
                throw new InvalidOperationException($"Cycle detected in execution plan at '{item.WorkItemId}'.");
        }
    }
}
