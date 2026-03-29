using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Core;
using HostPilot.Core.Providers.Models;
using HostPilot.Core.Providers.Parsers;

namespace HostPilot.Core.Providers.Adapters;

public sealed class SourceEngineProvider : ProviderAdapterBase
{
    public override string Id => "source-engine";
    public override string DisplayName => "Source Engine Dedicated Server";
    public override int? SteamAppId => 232330;
    public override IProviderLogParser LogParser { get; } = new SourceLogParser();

    public override ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile)
    {
        var map = GetString(profile, "map", "de_dust2");
        var game = GetString(profile, "game", "csgo");
        var maxPlayers = GetInt(profile, "maxPlayers", 16);
        var tickrate = GetInt(profile, "tickrate", 64);
        var exe = string.IsNullOrWhiteSpace(profile.ExecutablePath)
            ? Path.Combine(profile.InstallDirectory, "srcds.exe")
            : profile.ExecutablePath;
        var args = $"-game {game} -console -port {profile.GamePort} +map {map} +maxplayers {maxPlayers} -tickrate {tickrate}";
        return new ProviderCommand { Executable = exe, Arguments = args.Trim(), WorkingDirectory = Path.GetDirectoryName(exe) ?? profile.InstallDirectory };
    }

    public override IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile) =>
    [
        new() { CommandText = "writeid" },
        new() { CommandText = "quit" }
    ];
}
