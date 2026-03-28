using HostPilot.Core.Services.Discovery;

namespace HostPilot.Tests;

public class ManifestBackedServerDetectorTests
{
    [Fact]
    public void ScanPaths_FindsMatchingServerFootprint()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var installDir = Path.Combine(tempRoot, "steamapps", "common", "Valheim dedicated server");
        Directory.CreateDirectory(installDir);
        File.WriteAllText(Path.Combine(installDir, "valheim_server.exe"), string.Empty);
        File.WriteAllText(Path.Combine(installDir, "start_headless_server.bat"), string.Empty);

        try
        {
            var signature = new ServerDetectionSignature
            {
                ManifestId      = "valheim",
                DisplayName     = "Valheim",
                Executables     = ["valheim_server.exe"],
                Files           = ["start_headless_server.bat"],
                Folders         = ["Valheim dedicated server"],
                ConfigFiles     = ["start_headless_server.bat"],
                MinimumEvidence = 2,
            };

            var detector = new ManifestBackedServerDetector();
            var results = detector.ScanPaths([tempRoot], [signature]);

            var match = Assert.Single(results);
            Assert.Equal("valheim", match.ManifestId);
            Assert.Equal("valheim", match.ServerType);
            Assert.Contains(match.Evidence, e => e.Contains("Found executable", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    public void ScanPaths_ReturnsEmpty_WhenNoMatch()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);

        try
        {
            var signature = new ServerDetectionSignature
            {
                ManifestId      = "valheim",
                DisplayName     = "Valheim",
                Executables     = ["valheim_server.exe"],
                Files           = ["start_headless_server.bat"],
                MinimumEvidence = 2,
            };

            var detector = new ManifestBackedServerDetector();
            var results = detector.ScanPaths([tempRoot], [signature]);

            Assert.Empty(results);
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    public void ScanPaths_LoadsSignaturesFromRegistry_WhenDirectoryExists()
    {
        var sigDir = Path.Combine(AppContext.BaseDirectory, "DiscoverySignatures");
        var registry = new SignatureRegistry(sigDir);
        var signatures = registry.LoadAll();

        Assert.True(signatures.Count >= 28);
        Assert.Contains(signatures, s => s.ManifestId == "valheim");
        Assert.Contains(signatures, s => s.ManifestId == "minecraft-java");
    }
}
