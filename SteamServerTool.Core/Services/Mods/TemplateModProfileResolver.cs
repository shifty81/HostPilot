using System;
using SteamServerTool.Core.Models;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public sealed class TemplateModProfileResolver
{
    public ModSupportProfile Resolve(ServerTemplate template)
    {
        var game = template.Name?.Trim().ToLowerInvariant() ?? string.Empty;

        if (game.Contains("minecraft"))
        {
            return new ModSupportProfile
            {
                PrimaryProvider = ModProviderType.CurseForge,
                EnabledProviders = { ModProviderType.CurseForge, ModProviderType.Local },
                AcceptedLocalExtensions = { ".jar", ".zip" },
                AcceptFolders = false,
                AcceptZipArchives = true,
                InstallRootRelativePath = "mods",
                SupportsDependencies = true,
                SupportsBrowser = true,
                RequiresRestartAfterInstall = true,
            };
        }

        if (game.Contains("vintage"))
        {
            return new ModSupportProfile
            {
                PrimaryProvider = ModProviderType.VintageStory,
                EnabledProviders = { ModProviderType.VintageStory, ModProviderType.Local },
                AcceptedLocalExtensions = { ".zip" },
                AcceptFolders = true,
                InstallRootRelativePath = "Mods",
                SupportsDependencies = true,
                SupportsBrowser = true,
                RequiresRestartAfterInstall = true,
            };
        }

        return new ModSupportProfile
        {
            PrimaryProvider = ModProviderType.Local,
            EnabledProviders = { ModProviderType.Local },
            AcceptedLocalExtensions = { ".zip", ".jar", ".dll" },
            AcceptFolders = true,
            AcceptZipArchives = true,
            InstallRootRelativePath = "mods",
            SupportsDependencies = false,
            SupportsBrowser = false,
            RequiresRestartAfterInstall = true,
        };
    }
}
