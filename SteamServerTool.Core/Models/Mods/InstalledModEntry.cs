using System;

namespace SteamServerTool.Core.Models.Mods;

public sealed class InstalledModEntry
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public ModProviderType ProviderType { get; set; } = ModProviderType.Local;
    public string Status { get; set; } = "Installed";
    public bool IsDisabled { get; set; }
    public bool RequiresRestart { get; set; }
    public string? InstalledPath { get; set; }
    public DateTimeOffset? InstalledUtc { get; set; }
}
