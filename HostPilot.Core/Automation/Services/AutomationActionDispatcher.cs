using System;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Contracts;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Services;

public sealed class AutomationActionDispatcher : IAutomationActionDispatcher
{
    public Task<AutomationActionResult> DispatchAsync(
        AutomationRule rule,
        AutomationAction action,
        RuntimeEvent runtimeEvent,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Replace these branches with actual wiring into existing services:
        // - ServerManager
        // - BackupService
        // - RconClient
        // - WorkshopService
        // - notification center service
        var result = action.Kind switch
        {
            "Notify" => Success(action, $"Notification queued for rule '{rule.Name}'."),
            "Backup" => Success(action, $"Backup requested for server '{runtimeEvent.ServerId}'."),
            "Restart" => Success(action, $"Restart requested for server '{runtimeEvent.ServerId}'."),
            "Stop" => Success(action, $"Stop requested for server '{runtimeEvent.ServerId}'."),
            "Rcon" => Success(action, "RCON command requested."),
            _ => Failure(action, $"Unknown action kind '{action.Kind}'.")
        };

        return Task.FromResult(result);
    }

    private static AutomationActionResult Success(AutomationAction action, string message) =>
        new()
        {
            Order = action.Order,
            Kind = action.Kind,
            Succeeded = true,
            Message = message,
            TimestampUtc = DateTimeOffset.UtcNow
        };

    private static AutomationActionResult Failure(AutomationAction action, string message) =>
        new()
        {
            Order = action.Order,
            Kind = action.Kind,
            Succeeded = false,
            Message = message,
            TimestampUtc = DateTimeOffset.UtcNow
        };
}
