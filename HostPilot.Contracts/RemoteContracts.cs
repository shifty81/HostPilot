using System;
using System.Collections.Generic;

namespace HostPilot.Contracts;

public enum RemoteNodeStatus
{
    Unknown,
    Online,
    Degraded,
    Offline,
    Maintenance
}

public enum NodeCommandType
{
    Ping,
    Discover,
    Deploy,
    StartServer,
    StopServer,
    RestartServer,
    BackupServer,
    FetchLogs,
    InstallMods,
    ValidateServer,
    ExecuteAutomation,
    Custom
}

public enum OperationExecutionState
{
    Queued,
    Accepted,
    Running,
    Succeeded,
    Failed,
    Cancelled,
    TimedOut
}

public sealed class NodeCapabilitiesDto
{
    public bool SupportsSteamCmd { get; set; }
    public bool SupportsDocker { get; set; }
    public bool SupportsRcon { get; set; }
    public bool SupportsWorkshop { get; set; }
    public bool SupportsCurseForge { get; set; }
    public bool SupportsClustering { get; set; }
    public List<string> Providers { get; set; } = new();
    public List<string> InstalledRuntimes { get; set; } = new();
}

public sealed class RemoteNodeDto
{
    public string NodeId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public RemoteNodeStatus Status { get; set; }
    public DateTimeOffset LastHeartbeatUtc { get; set; }
    public NodeCapabilitiesDto Capabilities { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}

public sealed class NodeHeartbeatDto
{
    public string NodeId { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; }
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public long AvailableDiskBytes { get; set; }
    public int RunningServerCount { get; set; }
    public string AgentVersion { get; set; } = string.Empty;
}

public sealed class NodeCommandEnvelope
{
    public Guid CommandId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string NodeId { get; set; } = string.Empty;
    public string TargetServerId { get; set; } = string.Empty;
    public NodeCommandType CommandType { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTimeOffset RequestedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, string> Arguments { get; set; } = new();
}

public sealed class OperationStatusDto
{
    public Guid OperationId { get; set; }
    public Guid CorrelationId { get; set; }
    public OperationExecutionState State { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string TargetServerId { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public sealed class DashboardSummaryDto
{
    public int TotalNodes { get; set; }
    public int OnlineNodes { get; set; }
    public int OfflineNodes { get; set; }
    public int TotalServers { get; set; }
    public int RunningServers { get; set; }
    public int ActiveOperations { get; set; }
    public int ActiveAlerts { get; set; }
}

public sealed class PermissionGrantDto
{
    public string SubjectId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
