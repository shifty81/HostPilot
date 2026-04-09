namespace HostPilot.Remote.Contracts.Models;

public sealed class RemoteFileEntry
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public bool IsDirectory { get; set; }
    public long? SizeBytes { get; set; }
    public DateTimeOffset? LastModifiedUtc { get; set; }
}
