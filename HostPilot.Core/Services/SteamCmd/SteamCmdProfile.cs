namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdProfile
{
    public string ProfileName { get; set; } = string.Empty;
    public string InstallDirectory { get; set; } = string.Empty;
    public string AppId { get; set; } = "0";
    public string SteamCmdPath { get; set; } = "steamcmd";
    public string Branch { get; set; } = "public";

    // Server launch settings
    public string? ServerExePath { get; set; }
    public string ServerArguments { get; set; } = string.Empty;
    public int GamePort { get; set; } = 27015;

    // RCON
    public string Host { get; set; } = "127.0.0.1";
    public int RconPort { get; set; } = 27015;
    public string RconPassword { get; set; } = string.Empty;
    public int StopTimeoutSeconds { get; set; } = 25;
}

