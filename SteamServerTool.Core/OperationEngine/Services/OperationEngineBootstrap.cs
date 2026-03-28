using SteamServerTool.Core.Models;
using SteamServerTool.Core.OperationEngine.Abstractions;
using SteamServerTool.Core.OperationEngine.Adapters;
using SteamServerTool.Core.OperationEngine.Configuration;
using SteamServerTool.Core.OperationEngine.Models;
using SteamServerTool.Core.Services;

namespace SteamServerTool.Core.OperationEngine.Services;

public static class OperationEngineBootstrap
{
    public static OperationEngineService Create(
        ServerManager serverManager,
        SteamCmdService steamCmdService,
        IEnumerable<ServerConfig> serverConfigs,
        IEnumerable<ClusterDefinition>? clusters = null,
        OperationEngineOptions? options = null)
    {
        var stateStore = new InMemoryOperationStateStore();
        var eventBus = new InMemoryEventBus();
        var stateService = new ServerStateService(stateStore, eventBus, serverManager);

        foreach (var config in serverConfigs)
            stateService.Capture(config);

        var guards = new IOperationGuard[]
        {
            new ServerOperationGuard(stateStore),
        };

        var handlers = new List<IOperationHandler>
        {
            new InstallServerOperationHandler(steamCmdService, serverConfigs, stateService),
            new StartServerOperationHandler(serverManager, serverConfigs, stateService),
            new StopServerOperationHandler(serverManager, serverConfigs, stateService),
        };

        var engine = new OperationEngineService(
            options ?? new OperationEngineOptions(),
            stateStore,
            eventBus,
            guards,
            handlers);

        if (clusters is not null)
            engine.RegisterHandler(new ClusterStartOperationHandler(clusters, engine));

        return engine;
    }
}
