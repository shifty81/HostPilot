using HostPilot.Core.Providers.Abstractions;

namespace HostPilot.Core.Providers.Models;

public sealed class ParsedLogEvent
{
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
    public LogSeverity Severity { get; init; } = LogSeverity.Info;
    public string Category { get; init; } = "General";
    public string Message { get; init; } = "";
    public string RawLine { get; init; } = "";
    public int? ProgressPercent { get; init; }
    public bool IndicatesReady { get; init; }
    public bool IndicatesStopping { get; init; }
    public bool IndicatesStopped { get; init; }
    public bool IndicatesCrash { get; init; }
}
