namespace HostPilot.Remote.Files;

public sealed class RemoteFileEntry
{
    public string Name { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;
    public string RelativePath { get; init; } = string.Empty;
    public bool IsDirectory { get; init; }
    public long SizeBytes { get; init; }
    public DateTimeOffset ModifiedUtc { get; init; }
}

public sealed class RemoteDirectoryListing
{
    public string NodeId { get; init; } = string.Empty;
    public string RootAlias { get; init; } = string.Empty;
    public string CurrentPath { get; init; } = string.Empty;
    public IReadOnlyList<RemoteFileEntry> Entries { get; init; } = Array.Empty<RemoteFileEntry>();
}

public sealed class RemoteTransferProgress
{
    public string TransferId { get; init; } = string.Empty;
    public string NodeId { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public long BytesTransferred { get; init; }
    public long TotalBytes { get; init; }
    public string Stage { get; init; } = string.Empty;
}
