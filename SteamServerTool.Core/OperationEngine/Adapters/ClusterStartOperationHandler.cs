using SteamServerTool.Core.OperationEngine.Abstractions;
using SteamServerTool.Core.OperationEngine.Models;

namespace SteamServerTool.Core.OperationEngine.Adapters;

public sealed class ClusterStartOperationHandler(IEnumerable<ClusterDefinition> clusters, IOperationEngine operationEngine) : IOperationHandler
{
    public string OperationType => SteamServerTool.Core.OperationEngine.Models.OperationType.StartCluster;

    public async Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken)
    {
        var cluster = clusters.FirstOrDefault(x => string.Equals(x.ClusterId, context.TargetId, StringComparison.OrdinalIgnoreCase));
        if (cluster is null)
            return OperationResult.Failure($"Cluster '{context.TargetId}' was not found.");

        var queued = new List<string>();
        string? dependency = null;

        foreach (var nodeId in cluster.NodeServerIds)
        {
            var request = new OperationRequest
            {
                Type = SteamServerTool.Core.OperationEngine.Models.OperationType.StartServer,
                TargetId = nodeId,
                Priority = OperationPriority.High,
                DependsOnOperationIds = dependency is null ? Array.Empty<string>() : new[] { dependency },
                Metadata = new Dictionary<string, string>
                {
                    ["clusterId"] = cluster.ClusterId,
                    ["clusterName"] = cluster.Name,
                }
            };

            dependency = await operationEngine.QueueAsync(request, cancellationToken);
            queued.Add(dependency);
        }

        return OperationResult.Success($"Queued start for cluster '{cluster.Name}'.", queued);
    }
}
