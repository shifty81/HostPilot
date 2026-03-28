using SteamServerTool.Core.OperationEngine.Models;

namespace SteamServerTool.Core.OperationEngine.Events;

public sealed record ServerStateChangedEvent(ServerStateSnapshot Snapshot)
    : OperationEvent(DateTimeOffset.UtcNow);
