using HostPilot.Core.Models.Providers;
using HostPilot.Core.Services.Providers;
using Xunit;

namespace HostPilot.Tests;

public sealed class JsonProviderSettingsStoreTests
{
    [Fact]
    public async Task Saves_And_Loads_Settings()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "provider-settings.json");

        try
        {
            var store = new JsonProviderSettingsStore(path);
            await store.SaveAsync(new ProviderSettings { CurseForgeEnabled = false, DownloadCachePath = "CacheX" });
            var loaded = await store.LoadAsync();

            Assert.False(loaded.CurseForgeEnabled);
            Assert.Equal("CacheX", loaded.DownloadCachePath);
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
