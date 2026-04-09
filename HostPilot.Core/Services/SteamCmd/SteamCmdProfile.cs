namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdProfile
{
    public string ProfileName { get; set; } = string.Empty;
    public string InstallDirectory { get; set; } = string.Empty;
    public string AppId { get; set; } = "0";
    public string SteamCmdPath { get; set; } = "steamcmd";
    public string Branch { get; set; } = "public";
}
