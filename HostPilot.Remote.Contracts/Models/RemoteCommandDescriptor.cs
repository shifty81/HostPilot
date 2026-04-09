namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteCommandDescriptor
{
    public string CommandKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string HandlerKey { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public string[] RequiredPermissions { get; set; } = Array.Empty<string>();
    public bool SupportsCluster { get; set; }
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromMinutes(10);
}
