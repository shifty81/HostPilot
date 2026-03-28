using Xunit;
using SteamServerTool.Core.Models;
using SteamServerTool.Core.Models.Mods;
using SteamServerTool.Core.Services.Mods;

namespace SteamServerTool.Tests.Mods;

public sealed class ManagedModInstallCoordinatorTests
{
    [Fact]
    public async Task InstallCatalogItemAsync_WritesManagedMarkerFile()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            var server = new ServerConfig { Name = "Minecraft", Dir = root };
            var profile = new ModSupportProfile { InstallRootRelativePath = "mods" };
            var coordinator = new ManagedModInstallCoordinator();

            var entry = await coordinator.InstallCatalogItemAsync(server, profile, new ModCatalogItem
            {
                Id = "jei",
                Name = "JEI",
                ProviderType = ModProviderType.CurseForge,
                Version = "1.0.0"
            });

            Assert.NotNull(entry.InstalledPath);
            Assert.True(File.Exists(entry.InstalledPath!));
        }
        finally
        {
            if (Directory.Exists(root))
                Directory.Delete(root, true);
        }
    }
}
