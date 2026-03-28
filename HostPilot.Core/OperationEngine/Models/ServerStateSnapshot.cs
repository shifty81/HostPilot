namespace HostPilot.Core.OperationEngine.Models;

public sealed class ServerStateSnapshot
{
    public string ServerId { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public ServerRuntimeStatus Status { get; init; } = ServerRuntimeStatus.Unknown;
    public bool IsInstalled { get; init; }
    public bool IsRunning { get; init; }
    public double CpuPercent { get; init; }
    public long MemoryMb { get; init; }
    public int PlayerCount { get; init; }
    public string? ClusterId { get; init; }
    public string Health { get; init; } = "UNKNOWN";
    public string? LastError { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAtUtc { get; init; }
    public TimeSpan? Uptime { get; init; }
}
