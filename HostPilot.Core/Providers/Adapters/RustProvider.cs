using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Core;
using HostPilot.Core.Providers.Models;
using HostPilot.Core.Providers.Parsers;

namespace HostPilot.Core.Providers.Adapters;

public sealed class RustProvider : ProviderAdapterBase
{
    public override string Id => "rust";
    public override string DisplayName => "Rust Dedicated Server";
    public override int? SteamAppId => 258550;
    public override IProviderLogParser LogParser { get; } = new RustLogParser();

    public override ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile)
    {
        var worldSize = GetInt(profile, "worldSize", 3500);
        var seed = GetInt(profile, "worldSeed", 12345);
        var identity = GetString(profile, "serverIdentity", "default");
        var hostname = GetString(profile, "hostname", profile.DisplayName);
        var extra = GetString(profile, "extraArgs");
        var exe = string.IsNullOrWhiteSpace(profile.ExecutablePath)
            ? Path.Combine(profile.InstallDirectory, "RustDedicated.exe")
            : profile.ExecutablePath;

        var args = $"-batchmode +server.port {profile.GamePort} +server.queryport {profile.QueryPort} +rcon.port {profile.RconPort} +rcon.password \"{profile.RconPassword}\" +server.hostname \"{hostname}\" +server.identity \"{identity}\" +server.worldsize {worldSize} +server.seed {seed} {extra}";
        return new ProviderCommand { Executable = exe, Arguments = args.Trim(), WorkingDirectory = Path.GetDirectoryName(exe) ?? profile.InstallDirectory };
    }

    public override IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile) =>
    [
        new() { CommandText = "server.save" },
        new() { CommandText = "quit" }
    ];
}
