namespace HostPilot.Remote.Agent.Options;

public sealed class RemoteAgentOptions
{
    public const string SectionName = "RemoteAgent";

    public string NodeId { get; set; } = Environment.MachineName;
    public string DisplayName { get; set; } = Environment.MachineName;
    public string ControllerBaseUrl { get; set; } = "https://localhost:7001";
    public int HeartbeatSeconds { get; set; } = 15;
    public int HealthSampleSeconds { get; set; } = 10;
    public string EnvironmentName { get; set; } = "Production";
    public string[] Tags { get; set; } = Array.Empty<string>();
    public string ManagedRootPath { get; set; } = @"C:\HostPilot";
}
