using HostPilot.Core.Models;

namespace HostPilot.Core.Services.Deployment;

public static class ManifestTemplateMapper
{
    public static ServerTemplate ToTemplate(DeploymentManifest manifest)
    {
        var queryPort = manifest.Ports.FirstOrDefault(p =>
            p.Name.Contains("query", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Contains("game", StringComparison.OrdinalIgnoreCase))?.Default ?? 0;
        var rconPort = manifest.Ports.FirstOrDefault(p =>
            p.Name.Contains("rcon", StringComparison.OrdinalIgnoreCase))?.Default ?? queryPort;
        var maxPlayersField = manifest.Fields.FirstOrDefault(f =>
            f.Key.Equals("MaxPlayers", StringComparison.OrdinalIgnoreCase));
        _ = int.TryParse(maxPlayersField?.Default ?? "0", out var parsedMaxPlayers);

        return new ServerTemplate
        {
            Name             = manifest.DisplayName,
            Icon             = manifest.Icon,
            Description      = $"Manifest-backed template ({manifest.Id})",
            AppId            = manifest.AppId ?? 0,
            Executable       = manifest.Executable,
            LaunchArgs       = manifest.LaunchArgsTemplate,
            DefaultDir       = manifest.DefaultDir,
            RconPort         = rconPort,
            QueryPort        = queryPort,
            MaxPlayers       = parsedMaxPlayers,
            RequiresSteamCmd = string.Equals(manifest.Distribution, "steamcmd", StringComparison.OrdinalIgnoreCase),
            ConfigDir        = manifest.ConfigFiles.FirstOrDefault() ?? string.Empty,
            StdinStopCommand = manifest.Id.StartsWith("minecraft", StringComparison.OrdinalIgnoreCase) ? "stop"
                             : manifest.Id == "vintagestory" ? "/stop"
                             : string.Empty,
            Group            = manifest.Cluster?.Supported == true ? "Cluster Capable" : "General",
        };
    }
}
