using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Contracts;

public interface IAutomationActionDispatcher
{
    Task<AutomationActionResult> DispatchAsync(
        AutomationRule rule,
        AutomationAction action,
        RuntimeEvent runtimeEvent,
        CancellationToken cancellationToken = default);
}
