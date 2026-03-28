namespace HostPilot.Core.OperationEngine.Models;

public sealed class ClusterDefinition
{
    public string ClusterId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<string> NodeServerIds { get; init; } = Array.Empty<string>();
    public IReadOnlyDictionary<string, string> SharedConfiguration { get; init; } = new Dictionary<string, string>();
}
