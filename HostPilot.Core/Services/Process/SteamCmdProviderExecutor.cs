using HostPilot.Core.Services.SteamCmd;

namespace HostPilot.Core.Services.Process;

public sealed class SteamCmdProviderExecutor : IProviderExecutor
{
    private readonly SteamCmdRunner _runner = new();

    public async Task ExecuteInstallAsync(ProcessSupervisorProfile profile, IProgress<string> progress, CancellationToken cancellationToken)
    {
        var steamCmdProfile = BuildSteamCmdProfile(profile);
        var result = await _runner.RunAsync(steamCmdProfile, SteamCmdJobKind.Install,
            log => progress.Report(log.Message),
            (pct, msg) => { if (msg != null) progress.Report(msg); },
            cancellationToken);
        if (!result.Succeeded)
            throw new InvalidOperationException(result.Summary);
    }

    public async Task ExecuteUpdateAsync(ProcessSupervisorProfile profile, IProgress<string> progress, CancellationToken cancellationToken)
    {
        var steamCmdProfile = BuildSteamCmdProfile(profile);
        var result = await _runner.RunAsync(steamCmdProfile, SteamCmdJobKind.Update,
            log => progress.Report(log.Message),
            (pct, msg) => { if (msg != null) progress.Report(msg); },
            cancellationToken);
        if (!result.Succeeded)
            throw new InvalidOperationException(result.Summary);
    }

    public async Task ExecuteValidateAsync(ProcessSupervisorProfile profile, IProgress<string> progress, CancellationToken cancellationToken)
    {
        var steamCmdProfile = BuildSteamCmdProfile(profile);
        var result = await _runner.RunAsync(steamCmdProfile, SteamCmdJobKind.Validate,
            log => progress.Report(log.Message),
            (pct, msg) => { if (msg != null) progress.Report(msg); },
            cancellationToken);
        if (!result.Succeeded)
            throw new InvalidOperationException(result.Summary);
    }

    private static SteamCmdProfile BuildSteamCmdProfile(ProcessSupervisorProfile profile)
    {
        return new SteamCmdProfile
        {
            ProfileName = profile.DisplayName,
            InstallDirectory = profile.InstallRoot,
            AppId = profile.SteamAppId ?? "0",
        };
    }
}
