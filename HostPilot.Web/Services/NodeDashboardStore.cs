namespace HostPilot.Web.Services;

using HostPilot.Remote.Contracts.Models;

public sealed class NodeDashboardStore
{
    private readonly List<RemoteNodeIdentity> _nodes =
    [
        new RemoteNodeIdentity
        {
            NodeId = "node-local-01",
            DisplayName = "Local Test Node",
            MachineName = Environment.MachineName,
            Platform = Environment.OSVersion.ToString(),
            AgentVersion = "0.1.0",
            EnvironmentName = "Development",
            Tags = ["local", "debug"]
        }
    ];

    public IReadOnlyList<RemoteNodeIdentity> GetNodes() => _nodes;
}
