using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Contracts;

public interface IAutomationRuleStore
{
    Task<IReadOnlyList<AutomationRule>> GetRulesAsync(CancellationToken cancellationToken = default);
    Task SaveRulesAsync(IReadOnlyList<AutomationRule> rules, CancellationToken cancellationToken = default);
    Task AppendExecutionAsync(AutomationExecutionRecord record, CancellationToken cancellationToken = default);
}
