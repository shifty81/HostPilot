using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Events;

public sealed record ServerStateChangedEvent(ServerStateSnapshot Snapshot)
    : OperationEvent(DateTimeOffset.UtcNow);
