using HostPilot.Core.Models;
using HostPilot.Core.Services;
using Xunit;

namespace HostPilot.Tests;

public class SteamCmdServiceTests
{
    [Fact]
    public void IsSteamCmdInstalled_DefaultPath_ReturnsFalse()
    {
        var svc = new SteamCmdService();
        // Default "steamcmd" path should not exist in test environment.
        Assert.False(svc.IsSteamCmdInstalled());
    }

    [Fact]
    public void IsSteamCmdInstalled_ExplicitNonexistentPath_ReturnsFalse()
    {
        var svc = new SteamCmdService
        {
            SteamCmdPath = "/nonexistent/steamcmd"
        };
        Assert.False(svc.IsSteamCmdInstalled());
    }

    [Fact]
    public async Task InstallOrUpdateServer_InvalidAppId_ReturnsFalse()
    {
        var svc = new SteamCmdService();
        var config = new ServerConfig
        {
            Name       = "Test",
            AppId      = 0,
            Dir        = "/tmp/test",
            Executable = "test",
            Rcon       = new() { Port = 27015 }
        };

        var messages = new List<string>();
        var progress = new Progress<string>(msg => messages.Add(msg));

        var result = await svc.InstallOrUpdateServer(config, progress);

        Assert.False(result);
    }

    [Fact]
    public async Task InstallOrUpdateServer_NegativeAppId_ReturnsFalse()
    {
        var svc = new SteamCmdService();
        var config = new ServerConfig
        {
            Name       = "Test",
            AppId      = -1,
            Dir        = "/tmp/test",
            Executable = "test",
            Rcon       = new() { Port = 27015 }
        };

        var result = await svc.InstallOrUpdateServer(config);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateMod_InvalidAppId_ReturnsFalse()
    {
        var svc = new SteamCmdService();
        var config = new ServerConfig
        {
            Name       = "Test",
            AppId      = 0,
            Dir        = "/tmp/test",
            Executable = "test",
            Rcon       = new() { Port = 27015 }
        };

        var result = await svc.UpdateMod(config, 12345);

        Assert.False(result);
    }
}
