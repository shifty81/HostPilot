using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Events;

public sealed record OperationStatusChangedEvent(string OperationId, string Type, string TargetId, OperationStatus Status)
    : OperationEvent(DateTimeOffset.UtcNow);
