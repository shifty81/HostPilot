namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteCommandResult
{
    public string CommandId { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public DateTimeOffset CompletedAtUtc { get; set; }
    public IReadOnlyDictionary<string, string> OutputArtifacts { get; set; } = new Dictionary<string, string>();
}
