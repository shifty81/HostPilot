using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Core;
using HostPilot.Core.Providers.Models;
using HostPilot.Core.Providers.Parsers;

namespace HostPilot.Core.Providers.Adapters;

public sealed class PalworldProvider : ProviderAdapterBase
{
    public override string Id => "palworld";
    public override string DisplayName => "Palworld Dedicated Server";
    public override int? SteamAppId => 2394010;
    public override IProviderLogParser LogParser { get; } = new PalworldLogParser();

    public override ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile)
    {
        var usePerf = GetBool(profile, "usePerformanceMode", true);
        var extra = GetString(profile, "extraArgs");
        var exe = string.IsNullOrWhiteSpace(profile.ExecutablePath)
            ? Path.Combine(profile.InstallDirectory, "PalServer.exe")
            : profile.ExecutablePath;
        var perf = usePerf ? "-useperfthreads -NoAsyncLoadingThread -UseMultithreadForDS" : string.Empty;
        var args = $"-port={profile.GamePort} -players={GetInt(profile, "maxPlayers", 32)} {perf} {extra}";
        return new ProviderCommand { Executable = exe, Arguments = args.Trim(), WorkingDirectory = Path.GetDirectoryName(exe) ?? profile.InstallDirectory };
    }

    public override IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile) =>
    [
        new() { CommandText = "Save" },
        new() { CommandText = "Shutdown 1 Admin shutdown" }
    ];
}
