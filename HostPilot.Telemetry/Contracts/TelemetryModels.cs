namespace HostPilot.Telemetry.Contracts;

public sealed record NodeMetricPoint(
    string NodeId,
    DateTimeOffset TimestampUtc,
    double CpuUsagePercent,
    double MemoryUsagePercent,
    double DiskUsagePercent,
    double NetworkInKbps,
    double NetworkOutKbps,
    int RunningServers,
    int RunningJobs);

public sealed record WorkloadHealthSnapshot(
    string NodeId,
    string WorkloadId,
    string Status,
    string Summary,
    DateTimeOffset TimestampUtc);

public sealed record ClusterCapacitySnapshot(
    DateTimeOffset TimestampUtc,
    int OnlineNodeCount,
    int TotalCpuCores,
    int FreeCpuCores,
    int TotalMemoryMb,
    int FreeMemoryMb,
    int RunningJobs);

public sealed record ClusterAlert(
    string AlertId,
    string Severity,
    string Category,
    string Message,
    DateTimeOffset RaisedUtc,
    IReadOnlyDictionary<string, string> Metadata);
