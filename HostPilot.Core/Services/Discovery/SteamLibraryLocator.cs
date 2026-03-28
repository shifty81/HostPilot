namespace HostPilot.Core.Services.Discovery;

public static class SteamLibraryLocator
{
    public static IReadOnlyList<string> GetLikelyLibraryRoots()
    {
        var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType is DriveType.Fixed && d.IsReady))
        {
            var candidates = new[]
            {
                Path.Combine(drive.RootDirectory.FullName, "SteamLibrary"),
                Path.Combine(drive.RootDirectory.FullName, "Games"),
                Path.Combine(drive.RootDirectory.FullName, "Servers"),
                Path.Combine(drive.RootDirectory.FullName, "SteamCMD")
            };

            foreach (var candidate in candidates.Where(Directory.Exists))
            {
                results.Add(candidate);
            }
        }

        return results.ToList();
    }
}
