using System;
using System.Collections.Generic;

namespace HostPilot.Core.Automation.Models;

public sealed class AutomationRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string TargetScope { get; set; } = AutomationTargetScopes.SingleServer;
    public string TargetServerId { get; set; } = string.Empty;
    public string? TargetClusterId { get; set; }
    public AutomationTrigger Trigger { get; set; } = new();
    public List<AutomationCondition> Conditions { get; set; } = new();
    public List<AutomationAction> Actions { get; set; } = new();
    public TimeSpan? Cooldown { get; set; }
    public TimeSpan? DebounceWindow { get; set; }
    public bool SuppressDuplicateExecutions { get; set; } = true;
    public bool RequireMaintenanceWindow { get; set; }
    public List<MaintenanceWindow> MaintenanceWindows { get; set; } = new();
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}

public static class AutomationTargetScopes
{
    public const string SingleServer = "SingleServer";
    public const string ClusterParent = "ClusterParent";
    public const string ClusterChildren = "ClusterChildren";
    public const string EntireCluster = "EntireCluster";
    public const string RollingCluster = "RollingCluster";
}
