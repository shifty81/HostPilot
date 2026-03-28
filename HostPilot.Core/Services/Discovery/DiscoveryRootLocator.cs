namespace HostPilot.Core.Services.Discovery;

public sealed class DiscoveryRootLocator
{
    public IReadOnlyList<string> GetSuggestedRoots(string? serversBasePath = null)
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void AddIfExists(string? path)
        {
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                roots.Add(path);
            }
        }

        AddIfExists(serversBasePath);
        AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"));
        AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam"));
        AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Steam"));
        AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop"));
        AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents"));
        AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));

        foreach (var steamLibrary in SteamLibraryLocator.GetLikelyLibraryRoots())
        {
            AddIfExists(steamLibrary);
        }

        return roots.ToList();
    }
}
