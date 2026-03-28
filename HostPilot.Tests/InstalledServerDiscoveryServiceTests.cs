using HostPilot.Core.Services.Discovery;
using Xunit;

namespace HostPilot.Tests;

public sealed class InstalledServerDiscoveryServiceTests
{
    [Fact]
    public async Task Discovers_VintageStory_From_Signature()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var install = Path.Combine(root, "VintageStoryServer");
        Directory.CreateDirectory(install);
        File.WriteAllText(Path.Combine(install, "VintagestoryServer.exe"), string.Empty);
        File.WriteAllText(Path.Combine(install, "serverconfig.json"), "{}");

        try
        {
            var detector = InstalledServerDiscoveryService.CreateDefaultDetectors();
            var service = new InstalledServerDiscoveryService(detector);
            var results = await service.DiscoverAsync(new[] { root });

            Assert.Contains(results, x => x.ServerType == "vintagestory" && x.InstallPath == install);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
}
