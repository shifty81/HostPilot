using HostPilot.Core.Services.Process;
using HostPilot.Core.Services.SteamCmd;

namespace HostPilot.Core.Services;

/// <summary>
/// Facade that ties together SteamCMD installation, server process supervision,
/// and RCON communication into a single per-profile coordinator.
/// </summary>
public sealed class SteamServerCoordinator
{
    private readonly SteamCmdRunner _steamCmd;
    private readonly ServerProcessSupervisor _supervisor;

    public ServerRunState State => _supervisor.CurrentState;

    public SteamServerCoordinator(SteamCmdRunner steamCmd, ServerProcessSupervisor supervisor)
    {
        _steamCmd = steamCmd;
        _supervisor = supervisor;
    }

    public Task<SteamCmdRunResult> InstallAsync(
        SteamCmdProfile profile,
        Action<SteamCmdLogEntry>? onLog,
        Action<double?, string?>? onProgress,
        CancellationToken cancellationToken = default)
        => _steamCmd.RunAsync(profile, SteamCmdJobKind.Install, onLog ?? (_ => { }), onProgress ?? ((_, _) => { }), cancellationToken);

    public Task<SteamCmdRunResult> UpdateAsync(
        SteamCmdProfile profile,
        Action<SteamCmdLogEntry>? onLog,
        Action<double?, string?>? onProgress,
        CancellationToken cancellationToken = default)
        => _steamCmd.RunAsync(profile, SteamCmdJobKind.Update, onLog ?? (_ => { }), onProgress ?? ((_, _) => { }), cancellationToken);

    public Task<SteamCmdRunResult> ValidateAsync(
        SteamCmdProfile profile,
        Action<SteamCmdLogEntry>? onLog,
        Action<double?, string?>? onProgress,
        CancellationToken cancellationToken = default)
        => _steamCmd.RunAsync(profile, SteamCmdJobKind.Validate, onLog ?? (_ => { }), onProgress ?? ((_, _) => { }), cancellationToken);

    public Task StartServerAsync(
        SteamCmdProfile profile,
        Action<string>? onLog = null,
        CancellationToken cancellationToken = default)
        => _supervisor.StartAsync(profile, onLog, cancellationToken);

    public Task StopServerAsync(
        SteamCmdProfile profile,
        ServerStopMode stopMode = ServerStopMode.RconQuit,
        Action<string>? onLog = null,
        CancellationToken cancellationToken = default)
        => _supervisor.StopAsync(profile, stopMode, onLog, cancellationToken);

    public Task<RconResponse> SendRconAsync(
        SteamCmdProfile profile,
        string command,
        CancellationToken cancellationToken = default)
        => _supervisor.ExecuteRconAsync(profile, command, cancellationToken);
}
