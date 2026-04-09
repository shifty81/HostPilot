using Microsoft.AspNetCore.SignalR;

namespace HostPilot.Web.Hubs;

/// <summary>
/// SignalR hub that allows clients to subscribe to live telemetry for a specific
/// cluster or individual node.
/// </summary>
public sealed class ClusterTelemetryHub : Hub
{
    /// <summary>Adds the caller to the group that receives all events for <paramref name="clusterId"/>.</summary>
    public Task SubscribeCluster(string clusterId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"cluster:{clusterId}");

    /// <summary>Adds the caller to the group that receives all events for <paramref name="nodeId"/>.</summary>
    public Task SubscribeNode(string nodeId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"node:{nodeId}");
}
