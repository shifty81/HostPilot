using HostPilot.Remote.Agent.Options;
using HostPilot.Remote.Contracts.Models;
using Microsoft.Extensions.Options;

namespace HostPilot.Remote.Agent.Services;

public sealed class AgentNodeIdentityProvider
{
    private readonly RemoteAgentOptions _options;

    public AgentNodeIdentityProvider(IOptions<RemoteAgentOptions> options)
    {
        _options = options.Value;
    }

    public RemoteNodeIdentity GetIdentity()
    {
        return new RemoteNodeIdentity
        {
            NodeId = _options.NodeId,
            DisplayName = _options.DisplayName,
            MachineName = Environment.MachineName,
            Platform = Environment.OSVersion.ToString(),
            AgentVersion = typeof(AgentNodeIdentityProvider).Assembly.GetName().Version?.ToString() ?? "0.0.0",
            EnvironmentName = _options.EnvironmentName,
            Tags = _options.Tags,
        };
    }
}
