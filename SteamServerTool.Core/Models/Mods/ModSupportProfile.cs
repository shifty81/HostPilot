using System.Collections.Generic;

namespace SteamServerTool.Core.Models.Mods;

public sealed class ModSupportProfile
{
    public ModProviderType PrimaryProvider { get; set; } = ModProviderType.Local;
    public List<ModProviderType> EnabledProviders { get; set; } = new();
    public List<string> AcceptedLocalExtensions { get; set; } = new();
    public bool AcceptFolders { get; set; }
    public bool AcceptZipArchives { get; set; } = true;
    public string InstallRootRelativePath { get; set; } = "mods";
    public bool SupportsDependencies { get; set; }
    public bool SupportsBrowser { get; set; }
    public bool RequiresRestartAfterInstall { get; set; } = true;
    public string? RequiredLoader { get; set; }
    public string? RequiredGameVersion { get; set; }
}
