using System.Collections.Concurrent;
using SysProcess = System.Diagnostics.Process;
using SysProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace HostPilot.Core.Services.Process;

public sealed class ProcessSupervisor
{
    private readonly ConcurrentDictionary<string, ManagedProcess> _running = new();
    public event Action<string, ServerProcessState>? StateChanged;
    public event Action<string, ProcessOutputLine>? OutputReceived;

    public async Task StartAsync(ProcessSupervisorProfile profile, CancellationToken cancellationToken = default)
    {
        if (_running.ContainsKey(profile.ProfileId))
            return;

        var exePath = Path.Combine(profile.InstallRoot, profile.ServerExecutable);
        var psi = new SysProcessStartInfo
        {
            FileName = exePath,
            Arguments = profile.LaunchArguments,
            WorkingDirectory = profile.WorkingDirectory ?? profile.InstallRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new SysProcess { StartInfo = psi, EnableRaisingEvents = true };
        var managed = new ManagedProcess(profile, process);
        if (!_running.TryAdd(profile.ProfileId, managed))
            return;

        process.OutputDataReceived += (_, e) => Emit(profile.ProfileId, "stdout", e.Data);
        process.ErrorDataReceived += (_, e) => Emit(profile.ProfileId, "stderr", e.Data);
        process.Exited += async (_, _) => await OnExitedAsync(profile.ProfileId);

        StateChanged?.Invoke(profile.ProfileId, ServerProcessState.Starting);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        StateChanged?.Invoke(profile.ProfileId, ServerProcessState.Running);
        await Task.CompletedTask;
    }

    public async Task StopAsync(string profileId, CancellationToken cancellationToken = default)
    {
        if (!_running.TryGetValue(profileId, out var managed))
            return;

        StateChanged?.Invoke(profileId, ServerProcessState.Stopping);
        try
        {
            if (!managed.Process.HasExited)
            {
                managed.Process.Kill(entireProcessTree: true);
                await managed.Process.WaitForExitAsync(cancellationToken);
            }
        }
        catch { }
        _running.TryRemove(profileId, out _);
        StateChanged?.Invoke(profileId, ServerProcessState.Stopped);
    }

    public bool IsRunning(string profileId) => _running.ContainsKey(profileId);

    private void Emit(string profileId, string stream, string? data)
    {
        if (string.IsNullOrWhiteSpace(data)) return;
        OutputReceived?.Invoke(profileId, new ProcessOutputLine
        {
            Stream = stream,
            Text = data
        });
    }

    private async Task OnExitedAsync(string profileId)
    {
        if (!_running.TryRemove(profileId, out var managed))
            return;

        var crashed = managed.Process.ExitCode != 0;
        StateChanged?.Invoke(profileId, crashed ? ServerProcessState.Crashed : ServerProcessState.Stopped);
        if (crashed && managed.Profile.AutoRestartOnCrash)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, managed.Profile.RestartDelaySeconds)));
            try { await StartAsync(managed.Profile); } catch { }
        }
    }

    private sealed record ManagedProcess(ProcessSupervisorProfile Profile, SysProcess Process);
}
