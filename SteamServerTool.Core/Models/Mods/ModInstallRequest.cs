namespace SteamServerTool.Core.Models.Mods;

public sealed class ModInstallRequest
{
    public string ServerId { get; set; } = string.Empty;
    public string ModId { get; set; } = string.Empty;
    public string? Version { get; set; }
    public ModProviderType ProviderType { get; set; }
    public bool InstallDependencies { get; set; } = true;
    public bool ForceReplace { get; set; }
}
