namespace HostPilot.Core.OperationEngine.Models;

public enum OperationStatus
{
    Pending = 0,
    Queued = 1,
    Running = 2,
    Succeeded = 3,
    Failed = 4,
    Cancelled = 5,
    Skipped = 6,
    Retrying = 7
}
