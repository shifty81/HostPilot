using SysProcess = System.Diagnostics.Process;
using SysProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using System.Net.Http;
using HostPilot.Core.Models;

namespace HostPilot.Core.Services;

public class SteamCmdService
{
    // Official Valve SteamCMD download URLs
    private const string WindowsDownloadUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
    private const string LinuxDownloadUrl   = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz";

    public string SteamCmdPath { get; set; } = "steamcmd";

    /// <summary>
    /// Returns true when the configured SteamCMD executable can be found on disk or in PATH.
    /// </summary>
    public bool IsSteamCmdInstalled()
    {
        // Check absolute/relative path first
        if (File.Exists(SteamCmdPath)) return true;

        // Check PATH
        var envPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        var ext     = OperatingSystem.IsWindows() ? ".exe" : "";
        var exeName = Path.GetFileName(SteamCmdPath) is { Length: > 0 } n ? n : "steamcmd";
        if (!exeName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            exeName += ext;

        return envPath
            .Split(Path.PathSeparator)
            .Any(dir => File.Exists(Path.Combine(dir, exeName)));
    }

    /// <summary>
    /// Downloads and extracts SteamCMD into <paramref name="installDir"/>.
    /// Sets <see cref="SteamCmdPath"/> to the newly installed executable on success.
    /// </summary>
    public async Task<bool> DownloadSteamCmdAsync(string installDir, IProgress<string>? progress = null)
    {
        try
        {
            Directory.CreateDirectory(installDir);

            bool isWindows = OperatingSystem.IsWindows();
            var url        = isWindows ? WindowsDownloadUrl : LinuxDownloadUrl;
            var archiveName = isWindows ? "steamcmd.zip" : "steamcmd_linux.tar.gz";
            var archivePath = Path.Combine(installDir, archiveName);

            AppLogger.Info($"Downloading SteamCMD from {url} to '{installDir}'.");
            progress?.Report($"Downloading SteamCMD from {url} …");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "HostPilot/1.0");
                var bytes = await client.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(archivePath, bytes);
            }

            AppLogger.Info("SteamCMD archive downloaded; extracting …");
            progress?.Report("Extracting SteamCMD (existing files will be overwritten) …");

            if (isWindows)
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, installDir, overwriteFiles: true);
                SteamCmdPath = Path.Combine(installDir, "steamcmd.exe");
            }
            else
            {
                // On Linux use tar
                var tar = new SysProcessStartInfo("tar", $"-xzf \"{archivePath}\" -C \"{installDir}\"")
                {
                    UseShellExecute        = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    CreateNoWindow         = true
                };
                using var proc = SysProcess.Start(tar)!;
                await proc.WaitForExitAsync();
                SteamCmdPath = Path.Combine(installDir, "steamcmd.sh");
            }

            // Clean up archive
            try { File.Delete(archivePath); } catch { /* best effort */ }

            AppLogger.Info($"SteamCMD installed to '{installDir}'; executable: '{SteamCmdPath}'.");
            progress?.Report($"SteamCMD installed to: {installDir}");
            return true;
        }
        catch (Exception ex)
        {
            AppLogger.Error($"SteamCMD download failed: {ex}");
            progress?.Report($"[ERROR] SteamCMD download failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InstallOrUpdateServer(ServerConfig config, IProgress<string>? progress = null)
    {
        if (config.AppId <= 0)
        {
            AppLogger.Error($"InstallOrUpdateServer: invalid AppID ({config.AppId}) for '{config.Name}'.");
            progress?.Report("Error: Invalid AppID.");
            return false;
        }

        AppLogger.Info($"Starting SteamCMD install/update for '{config.Name}' (AppID {config.AppId}).");
        Directory.CreateDirectory(config.Dir);

        var args = BuildInstallArgs(config);
        return await RunSteamCmd(args, progress);
    }

    public async Task<bool> UpdateMod(ServerConfig config, long modId, IProgress<string>? progress = null)
    {
        if (config.AppId <= 0)
        {
            AppLogger.Error($"UpdateMod: invalid AppID ({config.AppId}) for '{config.Name}'.");
            progress?.Report("Error: Invalid AppID.");
            return false;
        }

        AppLogger.Info($"Downloading Workshop mod {modId} for AppID {config.AppId}.");
        var args = $"+login anonymous +workshop_download_item {config.AppId} {modId} +quit";
        return await RunSteamCmd(args, progress);
    }

    private string BuildInstallArgs(ServerConfig config)
    {
        return $"+login anonymous +force_install_dir \"{config.Dir}\" +app_update {config.AppId} validate +quit";
    }

    private async Task<bool> RunSteamCmd(string args, IProgress<string>? progress)
    {
        // SteamCMD's first launch after install (or a long gap) always triggers a
        // self-update that exits with a non-zero code (commonly 7) before the
        // requested action actually runs.  Re-running the same command immediately
        // after the self-update succeeds reliably, so we retry once automatically.
        const int maxAttempts = 2;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            AppLogger.Info($"Running SteamCMD (attempt {attempt}): {SteamCmdPath} {args}");
            var psi = new SysProcessStartInfo
            {
                FileName = SteamCmdPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new SysProcess { StartInfo = psi };

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null)
                {
                    AppLogger.Info($"[SteamCMD] {e.Data}");
                    progress?.Report(e.Data);
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null)
                {
                    AppLogger.Warn($"[SteamCMD ERR] {e.Data}");
                    progress?.Report($"[ERR] {e.Data}");
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    AppLogger.Info($"SteamCMD finished with exit code 0 (OK) on attempt {attempt}.");
                    return true;
                }

                AppLogger.Warn($"SteamCMD exited with code {process.ExitCode} on attempt {attempt}.");

                if (attempt < maxAttempts)
                {
                    progress?.Report($"SteamCMD exited with code {process.ExitCode} (likely self-update). Retrying…");
                }
                else
                {
                    AppLogger.Error($"SteamCMD failed after {maxAttempts} attempt(s) with exit code {process.ExitCode}.");
                    progress?.Report($"SteamCMD failed with exit code {process.ExitCode} after {maxAttempts} attempt(s).");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Failed to run SteamCMD: {ex}");
                progress?.Report($"Error running steamcmd: {ex.Message}");
                return false;
            }
        }

        return false;
    }
}
