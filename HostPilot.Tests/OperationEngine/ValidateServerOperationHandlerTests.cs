using HostPilot.Core.OperationEngine.Adapters;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.Services.SteamCmd;
using Xunit;

namespace HostPilot.Tests.OperationEngine;

public sealed class ValidateServerOperationHandlerTests
{
    private static OperationContext MakeContext(string targetId = "test-server")
        => new(Guid.NewGuid().ToString("N"), OperationType.ValidateServer, targetId);

    private readonly ValidateServerOperationHandler _handler = new(new SteamCmdRunner());

    [Fact]
    public void OperationType_IsValidateServer()
    {
        Assert.Equal(OperationType.ValidateServer, _handler.OperationType);
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingInstallDir_ReturnsFailure()
    {
        var context = MakeContext();
        var payload = new Dictionary<string, object?>
        {
            ["installDir"] = @"C:\DoesNotExist\ServerInstall",
            ["appId"] = "12345",
        };

        var result = await _handler.ExecuteAsync(context, payload, CancellationToken.None);

        Assert.False(result.SuccessValue);
        Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteAsync_WithMissingAppId_ReturnsFailure()
    {
        var context = MakeContext();
        var payload = new Dictionary<string, object?>
        {
            ["installDir"] = Directory.GetCurrentDirectory(),
            ["appId"] = null,
        };

        var result = await _handler.ExecuteAsync(context, payload, CancellationToken.None);

        Assert.False(result.SuccessValue);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoPayload_ReturnsFailure()
    {
        var context = MakeContext();
        var payload = new Dictionary<string, object?>();

        var result = await _handler.ExecuteAsync(context, payload, CancellationToken.None);

        Assert.False(result.SuccessValue);
    }
}
