using System.Diagnostics;
using HostPilot.Core.Services.SteamCmd;
using SysProcess = System.Diagnostics.Process;

namespace HostPilot.Core.Services.Process;

/// <summary>
/// Supervises a running server process with per-profile state tracking,
/// RCON-based or forced stop, and output forwarding.
/// </summary>
public sealed class ServerProcessSupervisor
{
    private SysProcess? _currentProcess;

    public ServerRunState CurrentState { get; } = new();

    public async Task StartAsync(SteamCmdProfile profile, Action<string>? onLog, CancellationToken cancellationToken)
    {
        if (CurrentState.IsRunning)
        {
            onLog?.Invoke("Server is already running.");
            return;
        }

        var exePath = profile.ServerExePath;
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            onLog?.Invoke($"Server executable not found: {exePath}");
            return;
        }

        var psi = new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = profile.ServerArguments,
            WorkingDirectory = Path.GetDirectoryName(exePath) ?? AppContext.BaseDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        _currentProcess = new SysProcess { StartInfo = psi, EnableRaisingEvents = true };
        _currentProcess.Exited += (_, _) =>
        {
            CurrentState.IsRunning = false;
            CurrentState.StoppedUtc = DateTime.UtcNow;
            CurrentState.StatusText = "Stopped";
        };

        _currentProcess.Start();
        CurrentState.IsRunning = true;
        CurrentState.ProcessId = _currentProcess.Id;
        CurrentState.StartedUtc = DateTime.UtcNow;
        CurrentState.StatusText = "Running";

        onLog?.Invoke($"Started server PID {_currentProcess.Id}.");

        _ = PumpOutputAsync(_currentProcess.StandardOutput, onLog, cancellationToken);
        _ = PumpOutputAsync(_currentProcess.StandardError, onLog, cancellationToken);
        await Task.CompletedTask;
    }

    public async Task StopAsync(SteamCmdProfile profile, ServerStopMode stopMode, Action<string>? onLog, CancellationToken cancellationToken)
    {
        if (_currentProcess == null || _currentProcess.HasExited)
        {
            CurrentState.IsRunning = false;
            CurrentState.StatusText = "Stopped";
            return;
        }

        if (stopMode == ServerStopMode.RconQuit)
        {
            var rconResponse = await ExecuteRconAsync(profile, "quit", cancellationToken);
            onLog?.Invoke(rconResponse.Succeeded ? rconResponse.ResponseText : rconResponse.Summary);
        }
        else if (stopMode == ServerStopMode.CtrlC)
        {
            try { _currentProcess.CloseMainWindow(); }
            catch (Exception ex) { onLog?.Invoke(ex.Message); }
        }

        var timeout = TimeSpan.FromSeconds(Math.Max(5, profile.StopTimeoutSeconds));
        var deadline = DateTime.UtcNow + timeout;
        while (!_currentProcess.HasExited && DateTime.UtcNow < deadline)
        {
            await Task.Delay(500, cancellationToken);
        }

        if (!_currentProcess.HasExited)
        {
            _currentProcess.Kill(entireProcessTree: true);
            onLog?.Invoke("Forced server process termination after timeout.");
        }

        CurrentState.IsRunning = false;
        CurrentState.StoppedUtc = DateTime.UtcNow;
        CurrentState.StatusText = "Stopped";
        onLog?.Invoke("Server stopped.");
    }

    public async Task<RconResponse> ExecuteRconAsync(SteamCmdProfile profile, string command, CancellationToken cancellationToken)
    {
        using var rcon = new RconClient(profile.Host, profile.RconPort, profile.RconPassword);
        try
        {
            var connected = await rcon.ConnectAsync();
            if (!connected)
                return new RconResponse { Succeeded = false, Summary = "RCON authentication failed." };

            var responseText = await rcon.SendCommandAsync(command);
            return new RconResponse { Succeeded = true, Summary = $"RCON '{command}' executed.", ResponseText = responseText };
        }
        catch (Exception ex)
        {
            return new RconResponse { Succeeded = false, Summary = ex.Message };
        }
    }

    private static async Task PumpOutputAsync(StreamReader reader, Action<string>? onLog, CancellationToken cancellationToken)
    {
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null) break;
            onLog?.Invoke(line);
        }
    }
}

