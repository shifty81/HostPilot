using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Services;

public sealed class AutomationPolicyGuard
{
    public PolicyDecision Evaluate(AutomationRule rule, RuntimeEvent runtimeEvent)
    {
        if (!rule.IsEnabled)
        {
            return PolicyDecision.Reject("Rule is disabled.");
        }

        if (rule.RequireMaintenanceWindow && rule.MaintenanceWindows.Count == 0)
        {
            return PolicyDecision.Reject("Rule requires a maintenance window but none is configured.");
        }

        return PolicyDecision.Allow();
    }
}

public sealed record PolicyDecision(bool Allowed, string Message)
{
    public static PolicyDecision Allow() => new(true, string.Empty);
    public static PolicyDecision Reject(string message) => new(false, message);
}
