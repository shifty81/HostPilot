using HostPilot.Core.Services.Deployment;

namespace HostPilot.Tests;

public class DeploymentManifestRegistryTests
{
    [Fact]
    public void LoadAll_LoadsAllManifests_WhenDirectoryExists()
    {
        var manifestDir = Path.Combine(AppContext.BaseDirectory, "DeploymentManifests");
        var registry = new DeploymentManifestRegistry(manifestDir);

        var manifests = registry.LoadAll();

        Assert.True(manifests.Count >= 28);
        Assert.Contains(manifests, m => m.Id == "minecraft-java");
        Assert.Contains(manifests, m => m.Id == "vintagestory");
        Assert.Contains(manifests, m => m.Id == "valheim");
        Assert.Contains(manifests, m => m.Id == "ark-asa");
    }

    [Fact]
    public void LoadAll_ReturnsEmpty_WhenDirectoryMissing()
    {
        var registry = new DeploymentManifestRegistry(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        Assert.Empty(registry.LoadAll());
    }

    [Fact]
    public void LoadById_ReturnsManifest_WhenExists()
    {
        var manifestDir = Path.Combine(AppContext.BaseDirectory, "DeploymentManifests");
        var registry = new DeploymentManifestRegistry(manifestDir);

        var manifest = registry.LoadById("valheim");

        Assert.NotNull(manifest);
        Assert.Equal("valheim", manifest.Id);
        Assert.Equal("Valheim", manifest.DisplayName);
    }

    [Fact]
    public void LoadById_ReturnsNull_WhenNotFound()
    {
        var manifestDir = Path.Combine(AppContext.BaseDirectory, "DeploymentManifests");
        var registry = new DeploymentManifestRegistry(manifestDir);

        Assert.Null(registry.LoadById("nonexistent-game-xyz"));
    }

    [Fact]
    public void GetTemplates_ReturnsManifestBackedTemplates()
    {
        var manifestDir = Path.Combine(AppContext.BaseDirectory, "DeploymentManifests");
        var registry = new DeploymentManifestRegistry(manifestDir);
        var catalog = new DeploymentTemplateCatalog(registry);

        var templates = catalog.GetTemplates();

        Assert.True(templates.Count >= 28);
        Assert.Contains(templates, t => t.Name.Contains("Valheim", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(templates, t => t.Name.Contains("Minecraft", StringComparison.OrdinalIgnoreCase));
    }
}
