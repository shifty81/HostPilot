using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Core;
using HostPilot.Core.Providers.Models;
using HostPilot.Core.Providers.Parsers;

namespace HostPilot.Core.Providers.Adapters;

public sealed class ValheimProvider : ProviderAdapterBase
{
    public override string Id => "valheim";
    public override string DisplayName => "Valheim Dedicated Server";
    public override int? SteamAppId => 896660;
    public override IProviderLogParser LogParser { get; } = new ValheimLogParser();

    public override ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile)
    {
        var world = GetString(profile, "worldName", "Dedicated");
        var name = GetString(profile, "serverName", profile.DisplayName);
        var password = GetString(profile, "serverPassword", "changeme");
        var saveDir = GetString(profile, "saveDir");
        var extra = GetString(profile, "extraArgs");
        var exe = string.IsNullOrWhiteSpace(profile.ExecutablePath)
            ? Path.Combine(profile.InstallDirectory, "valheim_server.exe")
            : profile.ExecutablePath;

        var args = $"-name \"{name}\" -port {profile.GamePort} -world \"{world}\" -password \"{password}\" {(string.IsNullOrWhiteSpace(saveDir) ? string.Empty : $"-savedir \"{saveDir}\"")} {extra}";
        return new ProviderCommand { Executable = exe, Arguments = args.Trim(), WorkingDirectory = Path.GetDirectoryName(exe) ?? profile.InstallDirectory };
    }

    public override IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile)
        => Array.Empty<RconCommandPlan>();

    public override bool CanUseRconStop(ProviderDeploymentProfile profile) => false;
}
