using HostPilot.Core.Models.Mods;
using HostPilot.Core.Services.Mods;
using Xunit;

namespace HostPilot.Tests;

public class LocalModImportServiceTests : IDisposable
{
    private readonly string _tempDir;

    public LocalModImportServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"sst_mods_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public async Task PreviewAsync_JarFile_GeneratesCopyPlanToModsDirectory()
    {
        var sourceFile = Path.Combine(_tempDir, "jei.jar");
        await File.WriteAllTextAsync(sourceFile, "dummy");

        var request = new ModImportRequest
        {
            ServerName = "Minecraft One",
            ServerDirectory = Path.Combine(_tempDir, "server"),
            SourcePath = sourceFile,
            SourceType = ModImportSourceType.LocalFile,
            Rules = new ModIntakeRules
            {
                AcceptFiles = new() { ".jar", ".zip" },
                TargetPath = "mods",
                RequiresRestart = true,
            }
        };

        var service = new LocalModImportService();
        var plan = await service.PreviewAsync(request);

        var item = Assert.Single(plan.Items);
        Assert.Equal(ModInstallAction.CopyAsIs, item.Action);
        Assert.EndsWith(Path.Combine("mods", "jei.jar"), item.DestinationPath);
        Assert.True(plan.RequiresRestart);
    }

    [Fact]
    public async Task PreviewAsync_ZipFile_WithAutoExtract_GeneratesExtractPlan()
    {
        var sourceFile = Path.Combine(_tempDir, "pack.zip");
        await File.WriteAllTextAsync(sourceFile, "fakezip");

        var request = new ModImportRequest
        {
            ServerName = "Vintage Story",
            ServerDirectory = Path.Combine(_tempDir, "vs"),
            SourcePath = sourceFile,
            SourceType = ModImportSourceType.ZipArchive,
            Rules = new ModIntakeRules
            {
                AcceptFiles = new() { ".zip" },
                AutoExtractZip = true,
                TargetPath = "Mods",
            }
        };

        var service = new LocalModImportService();
        var plan = await service.PreviewAsync(request);

        var item = Assert.Single(plan.Items);
        Assert.Equal(ModInstallAction.ExtractArchive, item.Action);
    }

    [Fact]
    public async Task PreviewAsync_UnsupportedFile_RejectsWhenRawInstallDisabled()
    {
        var sourceFile = Path.Combine(_tempDir, "weird.dll");
        await File.WriteAllTextAsync(sourceFile, "dummy");

        var request = new ModImportRequest
        {
            ServerName = "Test",
            ServerDirectory = Path.Combine(_tempDir, "server"),
            SourcePath = sourceFile,
            Rules = new ModIntakeRules
            {
                AcceptFiles = new() { ".jar" },
                TargetPath = "mods",
                AllowRawInstall = false,
            }
        };

        var service = new LocalModImportService();
        var plan = await service.PreviewAsync(request);

        var item = Assert.Single(plan.Items);
        Assert.Equal(ModInstallAction.Reject, item.Action);
    }
}
