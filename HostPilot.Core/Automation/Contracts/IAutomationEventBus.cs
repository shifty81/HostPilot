using System;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Contracts;

public interface IAutomationEventBus
{
    event EventHandler<RuntimeEvent>? EventPublished;
    Task PublishAsync(RuntimeEvent runtimeEvent, CancellationToken cancellationToken = default);
}
