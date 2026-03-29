using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Core;
using HostPilot.Core.Providers.Models;
using HostPilot.Core.Providers.Parsers;

namespace HostPilot.Core.Providers.Adapters;

public sealed class ArkSurvivalAscendedProvider : ProviderAdapterBase
{
    public override string Id => "ark-survival-ascended";
    public override string DisplayName => "ARK: Survival Ascended";
    public override int? SteamAppId => 2430930;
    public override IProviderLogParser LogParser { get; } = new ArkLogParser();

    public override ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile)
    {
        var map = GetString(profile, "map", "TheIsland_WP");
        var session = GetString(profile, "sessionName", profile.DisplayName);
        var maxPlayers = GetInt(profile, "maxPlayers", 70);
        var extra = GetString(profile, "extraArgs");
        var exe = string.IsNullOrWhiteSpace(profile.ExecutablePath)
            ? Path.Combine(profile.InstallDirectory, "ShooterGame", "Binaries", "Win64", "ArkAscendedServer.exe")
            : profile.ExecutablePath;
        var args = $"{map}?listen?SessionName=\"{session}\"?Port={profile.GamePort}?QueryPort={profile.QueryPort}?MaxPlayers={maxPlayers} -servergamelog -NoBattlEye {extra}";
        return new ProviderCommand { Executable = exe, Arguments = args.Trim(), WorkingDirectory = Path.GetDirectoryName(exe) ?? profile.InstallDirectory };
    }

    public override IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile) =>
    [
        new() { CommandText = "SaveWorld" },
        new() { CommandText = "DoExit" }
    ];
}
