using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Core;

public abstract class ProviderAdapterBase : IGameProviderAdapter
{
    public abstract string Id { get; }
    public abstract string DisplayName { get; }
    public virtual string InstallChannel => "SteamCMD";
    public abstract int? SteamAppId { get; }
    public abstract IProviderLogParser LogParser { get; }

    public virtual ProviderCommand BuildInstallCommand(ProviderDeploymentProfile profile, string steamCmdPath)
        => BuildSteamCmdAppUpdate(profile, steamCmdPath, validate: false);

    public virtual ProviderCommand BuildUpdateCommand(ProviderDeploymentProfile profile, string steamCmdPath)
        => BuildSteamCmdAppUpdate(profile, steamCmdPath, validate: false);

    public virtual ProviderCommand BuildValidateCommand(ProviderDeploymentProfile profile, string steamCmdPath)
        => BuildSteamCmdAppUpdate(profile, steamCmdPath, validate: true);

    protected ProviderCommand BuildSteamCmdAppUpdate(ProviderDeploymentProfile profile, string steamCmdPath, bool validate)
    {
        if (SteamAppId is null)
            throw new InvalidOperationException($"Provider '{Id}' does not use SteamCMD.");

        var forcePlatform = profile.Values.TryGetValue("steamPlatform", out var platformValue)
            ? platformValue?.ToString()
            : null;
        var anon = profile.Values.TryGetValue("steamLogin", out var loginValue)
            ? loginValue?.ToString() ?? "anonymous"
            : "anonymous";
        var validateSegment = validate ? " validate" : string.Empty;
        var platformSegment = string.IsNullOrWhiteSpace(forcePlatform)
            ? string.Empty
            : $" +@sSteamCmdForcePlatformType {forcePlatform}";

        return new ProviderCommand
        {
            Executable = steamCmdPath,
            Arguments = $"+force_install_dir \"{profile.InstallDirectory}\" +login {anon}{platformSegment} +app_update {SteamAppId}{validateSegment} +quit",
            WorkingDirectory = Path.GetDirectoryName(steamCmdPath) ?? profile.InstallDirectory,
            RequiresSteamCmd = true
        };
    }

    public abstract ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile);
    public abstract IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile);
    public virtual bool CanUseRconStop(ProviderDeploymentProfile profile)
        => profile.RconPort > 0 && !string.IsNullOrWhiteSpace(profile.RconPassword);

    public virtual string BuildHumanSummary(ProviderDeploymentProfile profile)
        => $"{DisplayName} at {profile.InstallDirectory}";

    protected static string GetString(ProviderDeploymentProfile profile, string key, string fallback = "")
        => profile.Values.TryGetValue(key, out var value) ? value?.ToString() ?? fallback : fallback;

    protected static int GetInt(ProviderDeploymentProfile profile, string key, int fallback = 0)
    {
        if (!profile.Values.TryGetValue(key, out var value) || value is null)
            return fallback;
        return int.TryParse(value.ToString(), out var parsed) ? parsed : fallback;
    }

    protected static bool GetBool(ProviderDeploymentProfile profile, string key, bool fallback = false)
    {
        if (!profile.Values.TryGetValue(key, out var value) || value is null)
            return fallback;
        return bool.TryParse(value.ToString(), out var parsed) ? parsed : fallback;
    }
}
