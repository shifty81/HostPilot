using SysProcess = System.Diagnostics.Process;
using SysProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdRunner
{
    private readonly SteamCmdArgumentBuilder _argumentBuilder = new();

    public string SteamCmdPath { get; set; } = "steamcmd";

    public async Task<SteamCmdRunResult> RunAsync(
        SteamCmdProfile profile,
        SteamCmdJobKind job,
        Action<SteamCmdLogEntry> logCallback,
        Action<double?, string?> progressCallback,
        CancellationToken cancellationToken = default)
    {
        var args = _argumentBuilder.Build(profile, job);

        var psi = new SysProcessStartInfo
        {
            FileName = string.IsNullOrWhiteSpace(profile.SteamCmdPath) ? SteamCmdPath : profile.SteamCmdPath,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using var process = new SysProcess { StartInfo = psi };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null) return;
            logCallback(new SteamCmdLogEntry { Message = e.Data, IsError = false });
            progressCallback(null, e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null) return;
            logCallback(new SteamCmdLogEntry { Message = e.Data, IsError = true });
            progressCallback(null, e.Data);
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
                return SteamCmdRunResult.Success($"SteamCMD '{job}' completed for '{profile.ProfileName}'.");

            return SteamCmdRunResult.Failure(
                $"SteamCMD exited with code {process.ExitCode} during '{job}' for '{profile.ProfileName}'.");
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);

            return SteamCmdRunResult.Failure($"SteamCMD '{job}' was cancelled for '{profile.ProfileName}'.");
        }
        catch (Exception ex)
        {
            return SteamCmdRunResult.Failure(
                $"SteamCMD '{job}' failed for '{profile.ProfileName}': {ex.Message}");
        }
    }

}
