namespace HostPilot.Core.Models.Nodes;

public sealed class NodeServerInventoryItem
{
    public Guid DeploymentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string InstallPath { get; set; } = string.Empty;
    public string Status { get; set; } = "Unknown";
}
