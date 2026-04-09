namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdArgumentBuilder
{
    public string Build(SteamCmdProfile profile, SteamCmdJobKind job)
    {
        var installDir = profile.InstallDirectory.Replace("\"", "\\\"");
        var appId = profile.AppId;

        var betaFlag = !string.IsNullOrWhiteSpace(profile.Branch)
                       && !profile.Branch.Equals("public", StringComparison.OrdinalIgnoreCase)
            ? $" -beta {profile.Branch}"
            : string.Empty;

        return job switch
        {
            SteamCmdJobKind.Install =>
                $"+login anonymous +force_install_dir \"{installDir}\" +app_update {appId}{betaFlag} +quit",
            SteamCmdJobKind.Update =>
                $"+login anonymous +force_install_dir \"{installDir}\" +app_update {appId}{betaFlag} +quit",
            SteamCmdJobKind.Validate =>
                $"+login anonymous +force_install_dir \"{installDir}\" +app_update {appId}{betaFlag} validate +quit",
            _ => throw new ArgumentOutOfRangeException(nameof(job), job, null),
        };
    }
}
