using HostPilot.Remote.Contracts.Models;

namespace HostPilot.Remote.Agent.Services;

public sealed class AgentHealthSampler
{
    public NodeHeartbeat Sample(string nodeId)
    {
        var process = Environment.ProcessId;
        return new NodeHeartbeat
        {
            NodeId = nodeId,
            SentAtUtc = DateTimeOffset.UtcNow,
            CpuUsagePercent = 0,
            MemoryUsagePercent = 0,
            RunningServerCount = 0,
            RunningOperationCount = 0,
            Alerts = new[]
            {
                new NodeAlertSnapshot
                {
                    Severity = "Info",
                    Code = "AGENT_UP",
                    Message = $"Agent process {process} is running."
                }
            }
        };
    }
}
