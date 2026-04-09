namespace HostPilot.Remote.Contracts.Models;

public sealed class NodeAlertSnapshot
{
    public string Severity { get; set; } = "Info";
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
