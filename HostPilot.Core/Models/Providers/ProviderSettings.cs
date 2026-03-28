namespace HostPilot.Core.Models.Providers;

public sealed class ProviderSettings
{
    public bool CurseForgeEnabled { get; set; } = true;
    public string CurseForgeApiKeyName { get; set; } = "CurseForge.ApiKey";

    public bool VintageStoryEnabled { get; set; } = true;
    public string VintageStoryBaseUrl { get; set; } = "https://mods.vintagestory.at/api";

    public bool SteamWorkshopEnabled { get; set; } = true;
    public string SteamWorkshopLookupUrl { get; set; } = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";
    public string SteamCmdPathOverride { get; set; } = string.Empty;

    public string DownloadCachePath { get; set; } = "Cache\\Providers";
    public string TempDownloadPath { get; set; } = "Temp\\Providers";
    public int RequestTimeoutSeconds { get; set; } = 60;
    public int MaxConcurrentDownloads { get; set; } = 2;
}
