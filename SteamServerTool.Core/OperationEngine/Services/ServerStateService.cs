using SteamServerTool.Core.Models;
using SteamServerTool.Core.OperationEngine.Abstractions;
using SteamServerTool.Core.OperationEngine.Events;
using SteamServerTool.Core.OperationEngine.Models;
using SteamServerTool.Core.Services;

namespace SteamServerTool.Core.OperationEngine.Services;

public sealed class ServerStateService
{
    private readonly IOperationStateStore _stateStore;
    private readonly IEventBus _eventBus;
    private readonly ServerManager _serverManager;

    public ServerStateService(IOperationStateStore stateStore, IEventBus eventBus, ServerManager serverManager)
    {
        _stateStore = stateStore;
        _eventBus = eventBus;
        _serverManager = serverManager;
    }

    public ServerStateSnapshot Capture(ServerConfig config, string? clusterId = null, string? lastError = null)
    {
        var isRunning = _serverManager.IsRunning(config.Name);
        var status = isRunning
            ? ServerRuntimeStatus.Running
            : Directory.Exists(config.Dir)
                ? ServerRuntimeStatus.Stopped
                : ServerRuntimeStatus.NotInstalled;

        var snapshot = new ServerStateSnapshot
        {
            ServerId = config.Name,
            DisplayName = config.Name,
            ClusterId = clusterId,
            IsInstalled = Directory.Exists(config.Dir),
            IsRunning = isRunning,
            Status = status,
            CpuPercent = _serverManager.GetCpuPercent(config.Name),
            MemoryMb = _serverManager.GetMemoryMb(config.Name),
            Uptime = _serverManager.GetUptime(config.Name),
            StartedAtUtc = _serverManager.GetUptime(config.Name) is { } uptime ? DateTimeOffset.UtcNow - uptime : null,
            LastError = lastError,
            Health = isRunning ? "GOOD" : "IDLE",
            UpdatedAtUtc = DateTimeOffset.UtcNow,
        };

        _stateStore.UpsertServerState(snapshot);
        _eventBus.Publish(new ServerStateChangedEvent(snapshot));
        return snapshot;
    }

    public ServerStateSnapshot SetStatus(string serverId, ServerRuntimeStatus status, string? error = null)
    {
        _stateStore.TryGetServerState(serverId, out var existing);
        var snapshot = new ServerStateSnapshot
        {
            ServerId = serverId,
            DisplayName = existing?.DisplayName ?? serverId,
            ClusterId = existing?.ClusterId,
            IsInstalled = existing?.IsInstalled ?? false,
            IsRunning = status is ServerRuntimeStatus.Starting or ServerRuntimeStatus.Running or ServerRuntimeStatus.Stopping,
            Status = status,
            CpuPercent = existing?.CpuPercent ?? 0,
            MemoryMb = existing?.MemoryMb ?? 0,
            PlayerCount = existing?.PlayerCount ?? 0,
            Health = status is ServerRuntimeStatus.Crashed or ServerRuntimeStatus.FailedInstall or ServerRuntimeStatus.InvalidConfig or ServerRuntimeStatus.PortConflict ? "CRITICAL" : existing?.Health ?? "UNKNOWN",
            LastError = error,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
            StartedAtUtc = existing?.StartedAtUtc,
            Uptime = existing?.Uptime,
        };

        _stateStore.UpsertServerState(snapshot);
        _eventBus.Publish(new ServerStateChangedEvent(snapshot));
        return snapshot;
    }
}
