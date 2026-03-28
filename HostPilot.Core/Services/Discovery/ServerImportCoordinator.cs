using HostPilot.Core.Models;
using HostPilot.Core.Models.Discovery;

namespace HostPilot.Core.Services.Discovery;

public sealed class ServerImportCoordinator
{
    public ServerConfig BuildImportedConfig(DiscoveredServerCandidate candidate)
    {
        var executableRelativePath = MakeRelativeExecutablePath(candidate.InstallPath, candidate.ExecutablePath);
        var backupFolder = Path.Combine(candidate.InstallPath, "Backups");
        var notes = $"Imported existing server on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";

        if (candidate.Evidence.Count > 0)
        {
            notes += Environment.NewLine + string.Join(Environment.NewLine, candidate.Evidence.Select(x => $"- {x}"));
        }

        return new ServerConfig
        {
            Name = string.IsNullOrWhiteSpace(candidate.DisplayName)
                ? Path.GetFileName(candidate.InstallPath)
                : candidate.DisplayName,
            Dir = candidate.InstallPath,
            Executable = executableRelativePath,
            LaunchArgs = candidate.Metadata.TryGetValue("launchArgs", out var launchArgs) ? launchArgs : string.Empty,
            BackupFolder = backupFolder,
            Notes = notes,
            AutoStartOnLaunch = false,
            Favorite = false,
            Group = "Imported",
            ServerType = candidate.ServerType,
            IsRemote = false,
            ConfigDir = candidate.ConfigPath is not null
                ? Path.GetRelativePath(candidate.InstallPath, Path.GetDirectoryName(candidate.ConfigPath) ?? candidate.InstallPath)
                : string.Empty,
            Tags = new List<string>
            {
                "Imported",
                "External"
            }
        };
    }

    private static string MakeRelativeExecutablePath(string installPath, string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            return string.Empty;
        }

        try
        {
            var relative = Path.GetRelativePath(installPath, executablePath);
            return relative.StartsWith("..") ? executablePath : relative;
        }
        catch
        {
            return executablePath;
        }
    }
}
