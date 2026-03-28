namespace HostPilot.Core.OperationEngine.Models;

public enum ServerRuntimeStatus
{
    Unknown = 0,
    NotInstalled = 1,
    Installing = 2,
    Stopped = 3,
    Starting = 4,
    Running = 5,
    Stopping = 6,
    Updating = 7,
    Crashed = 8,
    FailedInstall = 9,
    InvalidConfig = 10,
    PortConflict = 11
}
