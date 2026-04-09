namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteExecutionResult
{
    public string CorrelationId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string CommandKey { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public string Summary { get; set; } = string.Empty;
    public DateTimeOffset CompletedUtc { get; set; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<string> ArtifactPaths { get; set; } = Array.Empty<string>();
}
