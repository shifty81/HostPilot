namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteNodeIdentity
{
    public string NodeId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string AgentVersion { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = string.Empty;
    public IReadOnlyList<string> Tags { get; set; } = Array.Empty<string>();
}
