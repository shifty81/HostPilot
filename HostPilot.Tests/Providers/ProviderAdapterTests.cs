using HostPilot.Core.Providers.Adapters;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Core;
using HostPilot.Core.Providers.Models;
using HostPilot.Core.Providers.Parsers;
using Xunit;

namespace HostPilot.Tests.Providers;

public class ProviderRegistryTests
{
    [Fact]
    public void DefaultRegistry_ContainsAllExpectedProviders()
    {
        var registry = new ProviderRegistry();
        var ids = registry.All.Select(p => p.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("valheim", ids);
        Assert.Contains("rust", ids);
        Assert.Contains("palworld", ids);
        Assert.Contains("ark-survival-ascended", ids);
        Assert.Contains("source-engine", ids);
    }

    [Fact]
    public void GetRequired_KnownProvider_Returns()
    {
        var registry = new ProviderRegistry();
        var provider = registry.GetRequired("valheim");
        Assert.NotNull(provider);
        Assert.Equal("valheim", provider.Id);
    }

    [Fact]
    public void GetRequired_UnknownProvider_Throws()
    {
        var registry = new ProviderRegistry();
        Assert.Throws<KeyNotFoundException>(() => registry.GetRequired("does-not-exist"));
    }
}

public class ProviderAdapterBuildCommandTests
{
    private static ProviderDeploymentProfile MakeProfile(string providerId)
        => new()
        {
            ProviderId = providerId,
            DisplayName = "Test Server",
            InstallDirectory = @"C:\Servers\Test",
            Host = "127.0.0.1",
            GamePort = 27015,
            QueryPort = 27016,
            RconPort = 27020,
            RconPassword = "secret",
            StopTimeoutSeconds = 30,
        };

    [Fact]
    public void ValheimProvider_BuildInstallCommand_UsesSteamCmd()
    {
        var provider = new ValheimProvider();
        var profile = MakeProfile("valheim");
        var cmd = provider.BuildInstallCommand(profile, @"C:\Tools\steamcmd.exe");

        Assert.Contains("+app_update", cmd.Arguments);
        Assert.True(cmd.RequiresSteamCmd);
    }

    [Fact]
    public void ValheimProvider_BuildStartCommand_ContainsPortAndWorld()
    {
        var provider = new ValheimProvider();
        var profile = MakeProfile("valheim");
        profile.Values["worldName"] = "MyWorld";
        profile.GamePort = 2456;

        var cmd = provider.BuildStartCommand(profile);
        Assert.Contains("-port 2456", cmd.Arguments);
        Assert.Contains("-world \"MyWorld\"", cmd.Arguments);
    }

    [Fact]
    public void ValheimProvider_CanUseRconStop_ReturnsFalse()
    {
        var provider = new ValheimProvider();
        var profile = MakeProfile("valheim");
        Assert.False(provider.CanUseRconStop(profile));
    }

    [Fact]
    public void RustProvider_BuildStartCommand_ContainsRconPassword()
    {
        var provider = new RustProvider();
        var profile = MakeProfile("rust");
        var cmd = provider.BuildStartCommand(profile);

        Assert.Contains("+rcon.password", cmd.Arguments);
    }

    [Fact]
    public void ArkProvider_BuildGracefulStopPlan_ContainsSaveWorldAndExit()
    {
        var provider = new ArkSurvivalAscendedProvider();
        var profile = MakeProfile("ark-survival-ascended");
        var plan = provider.BuildGracefulStopPlan(profile);

        Assert.Contains(plan, c => c.CommandText == "SaveWorld");
        Assert.Contains(plan, c => c.CommandText == "DoExit");
    }

    [Fact]
    public void PalworldProvider_BuildStartCommand_ContainsPort()
    {
        var provider = new PalworldProvider();
        var profile = MakeProfile("palworld");
        profile.GamePort = 8211;

        var cmd = provider.BuildStartCommand(profile);
        Assert.Contains("-port=8211", cmd.Arguments);
    }

    [Fact]
    public void SourceProvider_BuildStartCommand_ContainsGame()
    {
        var provider = new SourceEngineProvider();
        var profile = MakeProfile("source-engine");
        profile.Values["game"] = "cstrike";

        var cmd = provider.BuildStartCommand(profile);
        Assert.Contains("-game cstrike", cmd.Arguments);
    }
}

public class LogParserTests
{
    [Theory]
    [InlineData("Game server connected", true)]
    [InlineData("World saved", false)]
    [InlineData("Some unrecognised line", false)]
    public void ValheimParser_DetectsReady(string line, bool expectedReady)
    {
        var parser = new ValheimLogParser();
        var result = parser.Parse(line);
        Assert.Equal(expectedReady, result.IndicatesReady);
    }

    [Fact]
    public void ArkParser_DetectsReady()
    {
        var parser = new ArkLogParser();
        var result = parser.Parse("Server started accepting connections");
        Assert.True(result.IndicatesReady);
    }

    [Fact]
    public void RustParser_DetectsCrash()
    {
        var parser = new RustLogParser();
        var result = parser.Parse("NullReferenceException crash in game loop");
        Assert.True(result.IndicatesCrash);
    }

    [Fact]
    public void PalworldParser_DetectsShutdown()
    {
        var parser = new PalworldLogParser();
        var result = parser.Parse("Saving world before shutdown");
        Assert.True(result.IndicatesStopping);
    }

    [Fact]
    public void SourceParser_DetectsReady()
    {
        var parser = new SourceLogParser();
        var result = parser.Parse("Connection to Steam servers successful");
        Assert.True(result.IndicatesReady);
    }
}
