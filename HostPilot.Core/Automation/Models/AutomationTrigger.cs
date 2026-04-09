using System.Collections.Generic;

namespace HostPilot.Core.Automation.Models;

public sealed class AutomationTrigger
{
    public string EventType { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
}

public sealed class AutomationCondition
{
    public string Kind { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Left { get; set; } = string.Empty;
    public string? Right { get; set; }
}

public sealed class AutomationAction
{
    public string Kind { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
    public int Order { get; set; }
    public int DelaySeconds { get; set; }
    public bool ContinueOnFailure { get; set; }
}

public sealed class MaintenanceWindow
{
    public string Name { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = "America/New_York";
    public List<DayWindow> Days { get; set; } = new();
}

public sealed class DayWindow
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartLocalTime { get; set; } = "00:00";
    public string EndLocalTime { get; set; } = "23:59";
}
