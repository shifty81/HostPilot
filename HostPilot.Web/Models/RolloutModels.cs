namespace HostPilot.Web.Models;

public sealed class RolloutPlanDto
{
    public string RolloutId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string PackageVersion { get; init; } = string.Empty;
    public IReadOnlyList<RolloutWaveDto> Waves { get; init; } = Array.Empty<RolloutWaveDto>();
}

public sealed class RolloutWaveDto
{
    public string WaveId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Concurrency { get; init; }
    public bool RequiresApproval { get; init; }
    public IReadOnlyList<string> NodeIds { get; init; } = Array.Empty<string>();
}

public sealed class RolloutNodeStateDto
{
    public string RolloutId { get; init; } = string.Empty;
    public string NodeId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int Percent { get; init; }
    public DateTimeOffset UpdatedUtc { get; init; } = DateTimeOffset.UtcNow;
}
