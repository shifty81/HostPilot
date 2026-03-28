namespace HostPilot.Core.Models.Mods;

public sealed class SteamWorkshopInstallRequest
{
    public uint AppId { get; set; }
    public string PublishedFileId { get; set; } = string.Empty;
    public string SteamCmdPath { get; set; } = "steamcmd.exe";
    public string Login { get; set; } = "anonymous";
    public bool Validate { get; set; } = true;
}
