using Xunit;
using HostPilot.Core.Models.Discovery;
using HostPilot.Core.Services.Discovery;

namespace HostPilot.Tests;

public sealed class ServerImportCoordinatorTests
{
    [Fact]
    public void BuildImportedConfig_MapsDiscoveryCandidate_ToServerConfig()
    {
        var candidate = new DiscoveredServerCandidate
        {
            CandidateId = "candidate-1",
            ServerType = "vintagestory",
            DisplayName = "Vintage Story Server",
            InstallPath = Path.Combine("C:", "Servers", "VintageStory"),
            ExecutablePath = Path.Combine("C:", "Servers", "VintageStory", "VintagestoryServer.exe"),
            ConfigPath = Path.Combine("C:", "Servers", "VintageStory", "Data", "serverconfig.json"),
            Evidence = new List<string> { "Found executable", "Found serverconfig.json" }
        };

        var coordinator = new ServerImportCoordinator();
        var config = coordinator.BuildImportedConfig(candidate);

        Assert.Equal("Vintage Story Server", config.Name);
        Assert.Equal(candidate.InstallPath, config.Dir);
        Assert.Equal("VintagestoryServer.exe", config.Executable);
        Assert.Equal("vintagestory", config.ServerType);
        Assert.Equal("Imported", config.Group);
        Assert.Contains("Imported existing server", config.Notes);
        Assert.Contains("Found executable", config.Notes);
    }
}
