using SteamServerTool.Core.OperationEngine.Adapters;
using SteamServerTool.Core.OperationEngine.Configuration;
using SteamServerTool.Core.OperationEngine.Models;
using SteamServerTool.Core.OperationEngine.Services;
using Xunit;

namespace SteamServerTool.Tests.OperationEngine;

public class OperationEngineTests
{
    [Fact]
    public async Task Guard_Skips_Start_When_Server_Is_Already_Running()
    {
        var stateStore = new InMemoryOperationStateStore();
        stateStore.UpsertServerState(new ServerStateSnapshot
        {
            ServerId = "ark_01",
            IsInstalled = true,
            IsRunning = true,
            Status = ServerRuntimeStatus.Running,
        });

        var engine = new OperationEngineService(
            new OperationEngineOptions { MaxConcurrentOperations = 1, PollingDelayMilliseconds = 20 },
            stateStore,
            new InMemoryEventBus(),
            new[] { new ServerOperationGuard(stateStore) },
            Array.Empty<SteamServerTool.Core.OperationEngine.Abstractions.IOperationHandler>());

        var operationId = await engine.QueueAsync(new OperationRequest
        {
            Type = OperationType.StartServer,
            TargetId = "ark_01",
        });

        await Task.Delay(120);
        Assert.True(engine.TryGetOperation(operationId, out var record));
        Assert.NotNull(record);
        Assert.Equal(OperationStatus.Skipped, record!.Status);
    }
}
