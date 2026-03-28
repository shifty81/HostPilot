using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Models;
using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public sealed class ManagedModInstallCoordinator : IModInstallCoordinator
{
    public Task<IReadOnlyList<InstalledModEntry>> GetInstalledAsync(ServerConfig serverConfig, ModSupportProfile profile, CancellationToken cancellationToken = default)
    {
        var results = new List<InstalledModEntry>();

        foreach (var modId in serverConfig.Mods)
        {
            results.Add(new InstalledModEntry
            {
                Id = modId.ToString(),
                Name = modId.ToString(),
                ProviderType = profile.PrimaryProvider,
                Status = "Configured",
                RequiresRestart = profile.RequiresRestartAfterInstall,
            });
        }

        foreach (var modId in serverConfig.DisabledMods)
        {
            results.Add(new InstalledModEntry
            {
                Id = modId.ToString(),
                Name = modId.ToString(),
                ProviderType = profile.PrimaryProvider,
                Status = "Disabled",
                IsDisabled = true,
                RequiresRestart = profile.RequiresRestartAfterInstall,
            });
        }

        var installRoot = ResolveInstallRoot(serverConfig, profile);
        if (Directory.Exists(installRoot))
        {
            foreach (var file in Directory.EnumerateFiles(installRoot))
            {
                var fileName = Path.GetFileName(file);
                if (results.Any(x => string.Equals(x.Name, fileName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                results.Add(new InstalledModEntry
                {
                    Id = fileName,
                    Name = fileName,
                    ProviderType = ModProviderType.Local,
                    Status = "Detected",
                    InstalledPath = file,
                    RequiresRestart = profile.RequiresRestartAfterInstall,
                    InstalledUtc = File.GetLastWriteTimeUtc(file),
                });
            }
        }

        return Task.FromResult<IReadOnlyList<InstalledModEntry>>(results.OrderBy(x => x.Name).ToList());
    }

    public Task<InstalledModEntry> InstallCatalogItemAsync(ServerConfig serverConfig, ModSupportProfile profile, ModCatalogItem item, CancellationToken cancellationToken = default)
    {
        if (serverConfig == null) throw new ArgumentNullException(nameof(serverConfig));
        if (item == null) throw new ArgumentNullException(nameof(item));

        if (item.ProviderType == ModProviderType.SteamWorkshop && long.TryParse(item.Id, out var workshopId))
        {
            if (!serverConfig.Mods.Contains(workshopId) && !serverConfig.DisabledMods.Contains(workshopId))
                serverConfig.Mods.Add(workshopId);

            return Task.FromResult(new InstalledModEntry
            {
                Id = item.Id,
                Name = item.Name,
                Version = item.Version,
                ProviderType = item.ProviderType,
                Status = "Queued",
                RequiresRestart = profile.RequiresRestartAfterInstall,
                InstalledUtc = DateTimeOffset.UtcNow,
            });
        }

        var installRoot = ResolveInstallRoot(serverConfig, profile);
        Directory.CreateDirectory(installRoot);

        var safeName = MakeSafeFileName(string.IsNullOrWhiteSpace(item.Name) ? item.Id : item.Name);
        var markerPath = Path.Combine(installRoot, $"{safeName}.managed.txt");
        File.WriteAllText(markerPath,
            $"Provider={item.ProviderType}{Environment.NewLine}" +
            $"Id={item.Id}{Environment.NewLine}" +
            $"Name={item.Name}{Environment.NewLine}" +
            $"Version={item.Version}{Environment.NewLine}" +
            $"InstalledUtc={DateTimeOffset.UtcNow:O}{Environment.NewLine}");

        return Task.FromResult(new InstalledModEntry
        {
            Id = item.Id,
            Name = item.Name,
            Version = item.Version,
            ProviderType = item.ProviderType,
            Status = "Installed",
            RequiresRestart = profile.RequiresRestartAfterInstall,
            InstalledPath = markerPath,
            InstalledUtc = DateTimeOffset.UtcNow,
        });
    }

    private static string ResolveInstallRoot(ServerConfig serverConfig, ModSupportProfile profile)
    {
        var baseDir = string.IsNullOrWhiteSpace(serverConfig.Dir)
            ? AppDomain.CurrentDomain.BaseDirectory
            : serverConfig.Dir;

        return Path.GetFullPath(Path.Combine(baseDir, profile.InstallRootRelativePath));
    }

    private static string MakeSafeFileName(string value)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars())
            value = value.Replace(invalid, '_');
        return value;
    }
}
