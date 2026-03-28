using HostPilot.Core.OperationEngine.Adapters;
using HostPilot.Core.OperationEngine.Configuration;
using HostPilot.Core.OperationEngine.Models;
using HostPilot.Core.OperationEngine.Services;
using Xunit;

namespace HostPilot.Tests.OperationEngine;

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
            Array.Empty<HostPilot.Core.OperationEngine.Abstractions.IOperationHandler>());

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
