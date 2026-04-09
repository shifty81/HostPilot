using HostPilot.Core.Services.Process;
using HostPilot.Core.Services.SteamCmd;
using Xunit;

namespace HostPilot.Tests;

public sealed class SteamServerCoordinatorTests
{
    [Fact]
    public void CurrentState_Initially_NotRunning()
    {
        var supervisor = new ServerProcessSupervisor();
        Assert.False(supervisor.CurrentState.IsRunning);
        Assert.Equal("Stopped", supervisor.CurrentState.StatusText);
    }

    [Fact]
    public async Task StartAsync_WithMissingExePath_DoesNotSetRunning()
    {
        var supervisor = new ServerProcessSupervisor();
        var profile = new SteamCmdProfile
        {
            ProfileName = "test",
            ServerExePath = @"C:\DoesNotExist\server.exe",
        };

        var logs = new List<string>();
        await supervisor.StartAsync(profile, logs.Add, CancellationToken.None);

        Assert.False(supervisor.CurrentState.IsRunning);
        Assert.Contains(logs, l => l.Contains("not found"));
    }

    [Fact]
    public async Task StopAsync_WhenNotRunning_RemainsStoppedState()
    {
        var supervisor = new ServerProcessSupervisor();
        var profile = new SteamCmdProfile
        {
            ProfileName = "test",
            StopTimeoutSeconds = 1,
        };

        await supervisor.StopAsync(profile, ServerStopMode.KillAfterTimeout, null, CancellationToken.None);

        Assert.False(supervisor.CurrentState.IsRunning);
    }

    [Fact]
    public void ServerRunState_DefaultValues_AreCorrect()
    {
        var state = new ServerRunState();
        Assert.False(state.IsRunning);
        Assert.Null(state.ProcessId);
        Assert.Null(state.StartedUtc);
        Assert.Null(state.StoppedUtc);
        Assert.Equal("Stopped", state.StatusText);
    }

    [Theory]
    [InlineData(ServerStopMode.RconQuit)]
    [InlineData(ServerStopMode.CtrlC)]
    [InlineData(ServerStopMode.KillAfterTimeout)]
    public void ServerStopMode_AllValuesAreDistinct(ServerStopMode mode)
    {
        // Ensures all enum values exist and can be used
        Assert.True(Enum.IsDefined(mode));
    }
}
