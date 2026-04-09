namespace HostPilot.Remote.Files;

public sealed class RemotePathPolicy
{
    public string NormalizeUnderRoot(string rootPath, string requestedPath)
    {
        var combined = Path.GetFullPath(Path.Combine(rootPath, requestedPath ?? string.Empty));
        var normalizedRoot = Path.GetFullPath(rootPath);

        var rootWithSeparator = normalizedRoot.EndsWith(Path.DirectorySeparatorChar)
            ? normalizedRoot
            : normalizedRoot + Path.DirectorySeparatorChar;

        if (!combined.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase)
            && !combined.Equals(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Path traversal attempt blocked.");
        }

        return combined;
    }
}
