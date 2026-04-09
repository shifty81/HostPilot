using System;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Contracts;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Services;

public sealed class AutomationEventBus : IAutomationEventBus
{
    public event EventHandler<RuntimeEvent>? EventPublished;

    public Task PublishAsync(RuntimeEvent runtimeEvent, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EventPublished?.Invoke(this, runtimeEvent);
        return Task.CompletedTask;
    }
}
