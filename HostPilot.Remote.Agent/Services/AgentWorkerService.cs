using HostPilot.Remote.Agent.Options;
using Microsoft.Extensions.Options;

namespace HostPilot.Remote.Agent.Services;

public sealed class AgentWorkerService : BackgroundService
{
    private readonly ILogger<AgentWorkerService> _logger;
    private readonly RemoteAgentOptions _options;
    private readonly AgentNodeIdentityProvider _identityProvider;
    private readonly AgentHealthSampler _healthSampler;

    public AgentWorkerService(
        ILogger<AgentWorkerService> logger,
        IOptions<RemoteAgentOptions> options,
        AgentNodeIdentityProvider identityProvider,
        AgentHealthSampler healthSampler)
    {
        _logger = logger;
        _options = options.Value;
        _identityProvider = identityProvider;
        _healthSampler = healthSampler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var identity = _identityProvider.GetIdentity();
        _logger.LogInformation("Remote agent starting for node {NodeId} ({DisplayName})", identity.NodeId, identity.DisplayName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var heartbeat = _healthSampler.Sample(identity.NodeId);
            _logger.LogInformation(
                "Heartbeat {NodeId} cpu={Cpu} mem={Mem} servers={Servers} ops={Ops}",
                heartbeat.NodeId,
                heartbeat.CpuUsagePercent,
                heartbeat.MemoryUsagePercent,
                heartbeat.RunningServerCount,
                heartbeat.RunningOperationCount);

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(5, _options.HeartbeatSeconds)), stoppingToken);
        }
    }
}
