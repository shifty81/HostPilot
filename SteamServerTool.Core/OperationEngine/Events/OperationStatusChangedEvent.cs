using SteamServerTool.Core.OperationEngine.Models;

namespace SteamServerTool.Core.OperationEngine.Events;

public sealed record OperationStatusChangedEvent(string OperationId, string Type, string TargetId, OperationStatus Status)
    : OperationEvent(DateTimeOffset.UtcNow);
