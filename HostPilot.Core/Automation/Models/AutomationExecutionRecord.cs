using System;
using System.Collections.Generic;

namespace HostPilot.Core.Automation.Models;

public sealed class AutomationExecutionRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RuleId { get; set; }
    public Guid EventId { get; set; }
    public string ServerId { get; set; } = string.Empty;
    public DateTimeOffset StartedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? FinishedUtc { get; set; }
    public string Result { get; set; } = AutomationExecutionResults.Running;
    public string Summary { get; set; } = string.Empty;
    public List<AutomationActionResult> ActionResults { get; set; } = new();
}

public sealed class AutomationActionResult
{
    public int Order { get; set; }
    public string Kind { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
}

public static class AutomationExecutionResults
{
    public const string Running = "Running";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string Rejected = "Rejected";
    public const string Skipped = "Skipped";
}
