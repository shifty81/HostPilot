using HostPilot.Core.Models;
using HostPilot.Core.Models.Discovery;
using HostPilot.Core.Services.Deployment;

namespace HostPilot.Core.Services.Discovery;

public class DiscoveredServerImportFactory
{
    private readonly DeploymentManifestRegistry _manifestRegistry;

    public DiscoveredServerImportFactory(DeploymentManifestRegistry manifestRegistry)
    {
        _manifestRegistry = manifestRegistry;
    }

    public ServerConfig Create(DiscoveredServerCandidate candidate)
    {
        var manifest = _manifestRegistry.LoadById(candidate.ManifestId);
        var template = manifest is not null
            ? ManifestTemplateMapper.ToTemplate(manifest)
            : new ServerTemplate
            {
                Name       = candidate.DisplayName,
                Executable = Path.GetFileName(candidate.ExecutablePath),
                DefaultDir = Path.GetFileName(candidate.InstallPath),
            };

        var executableName = Path.GetFileName(candidate.ExecutablePath);

        return new ServerConfig
        {
            Name             = BuildImportedName(candidate.DisplayName, candidate.InstallPath),
            AppId            = template.AppId,
            Dir              = candidate.InstallPath,
            Executable       = string.IsNullOrWhiteSpace(executableName) ? template.Executable : executableName,
            LaunchArgs       = template.LaunchArgs,
            QueryPort        = template.QueryPort,
            MaxPlayers       = template.MaxPlayers,
            Group            = string.IsNullOrWhiteSpace(template.Group) ? "Imported" : template.Group,
            BackupFolder     = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups", SanitizeFolderName(candidate.DisplayName)),
            ConfigDir        = template.ConfigDir,
            ServerType       = manifest?.Id ?? string.Empty,
            StdinStopCommand = template.StdinStopCommand,
            Tags             = new List<string> { "Imported", $"Confidence:{candidate.Confidence:0.00}" },
            Notes            = BuildNotes(candidate),
            Rcon             = new RconConfig
            {
                Host     = template.RconHost,
                Port     = template.RconPort,
                Password = string.Empty,
            },
        };
    }

    private static string BuildImportedName(string displayName, string installPath)
    {
        var folder = Path.GetFileName(installPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        return string.IsNullOrWhiteSpace(folder) ? $"{displayName} (Imported)" : $"{displayName} ({folder})";
    }

    private static string BuildNotes(DiscoveredServerCandidate candidate)
    {
        var evidence = candidate.Evidence.Count == 0
            ? "No additional evidence captured."
            : string.Join(Environment.NewLine, candidate.Evidence.Select(e => $"- {e}"));

        return $"Imported from existing installation.{Environment.NewLine}" +
               $"Confidence: {candidate.Confidence:0.00}{Environment.NewLine}" +
               $"Path: {candidate.InstallPath}{Environment.NewLine}" +
               $"Evidence:{Environment.NewLine}{evidence}";
    }

    private static string SanitizeFolderName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(value.Select(ch => invalid.Contains(ch) ? '_' : ch));
    }
}
