namespace HostPilot.Remote.Files;

public sealed class RemoteFileBrowserService
{
    private readonly RemotePathPolicy _pathPolicy = new();

    public Task<RemoteDirectoryListing> ListAsync(string nodeId, string rootAlias, string rootPath, string requestedPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var current = _pathPolicy.NormalizeUnderRoot(rootPath, requestedPath);
        var dir = new DirectoryInfo(current);

        var entries = dir.Exists
            ? dir.EnumerateFileSystemInfos()
                .OrderByDescending(x => (x.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                .ThenBy(x => x.Name)
                .Select(x => new RemoteFileEntry
                {
                    Name = x.Name,
                    FullPath = x.FullName,
                    RelativePath = Path.GetRelativePath(rootPath, x.FullName),
                    IsDirectory = (x.Attributes & FileAttributes.Directory) == FileAttributes.Directory,
                    SizeBytes = x is FileInfo fi ? fi.Length : 0,
                    ModifiedUtc = x.LastWriteTimeUtc
                })
                .ToArray()
            : Array.Empty<RemoteFileEntry>();

        return Task.FromResult(new RemoteDirectoryListing
        {
            NodeId = nodeId,
            RootAlias = rootAlias,
            CurrentPath = current,
            Entries = entries
        });
    }
}
