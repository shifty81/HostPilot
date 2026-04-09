using System;
using System.Collections.Generic;

namespace HostPilot.Core.Automation.Models;

public sealed class RuntimeEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public string? ClusterId { get; set; }
    public DateTimeOffset OccurredUtc { get; set; } = DateTimeOffset.UtcNow;
    public string Source { get; set; } = string.Empty;
    public string Severity { get; set; } = RuntimeEventSeverities.Info;
    public Dictionary<string, string> Payload { get; set; } = new();
}

public static class RuntimeEventSeverities
{
    public const string Trace = "Trace";
    public const string Info = "Info";
    public const string Warning = "Warning";
    public const string Error = "Error";
    public const string Critical = "Critical";
}
