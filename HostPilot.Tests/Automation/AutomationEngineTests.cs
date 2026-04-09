using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Contracts;
using HostPilot.Core.Automation.Models;
using HostPilot.Core.Automation.Services;
using Xunit;

namespace HostPilot.Tests.Automation;

public sealed class AutomationEngineTests
{
    [Fact]
    public async Task Matching_rule_dispatches_actions()
    {
        var ruleStore = new InMemoryRuleStore(new List<AutomationRule>
        {
            new()
            {
                Name = "Restart on exit",
                Trigger = new AutomationTrigger { EventType = "ProcessExited" },
                TargetServerId = "server-1",
                Actions = new List<AutomationAction>
                {
                    new() { Kind = "Restart", Order = 1 }
                }
            }
        });

        var dispatcher = new TestDispatcher();
        var engine = new AutomationEngine(ruleStore, dispatcher, new AutomationPolicyGuard());

        await engine.HandleEventAsync(new RuntimeEvent
        {
            EventType = "ProcessExited",
            ServerId = "server-1"
        });

        Assert.Equal(1, dispatcher.DispatchCount);
    }

    private sealed class InMemoryRuleStore : IAutomationRuleStore
    {
        private readonly IReadOnlyList<AutomationRule> _rules;

        public InMemoryRuleStore(IReadOnlyList<AutomationRule> rules)
        {
            _rules = rules;
        }

        public Task<IReadOnlyList<AutomationRule>> GetRulesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_rules);

        public Task SaveRulesAsync(IReadOnlyList<AutomationRule> rules, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AppendExecutionAsync(AutomationExecutionRecord record, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class TestDispatcher : IAutomationActionDispatcher
    {
        public int DispatchCount { get; private set; }

        public Task<AutomationActionResult> DispatchAsync(
            AutomationRule rule,
            AutomationAction action,
            RuntimeEvent runtimeEvent,
            CancellationToken cancellationToken = default)
        {
            DispatchCount++;
            return Task.FromResult(new AutomationActionResult
            {
                Kind = action.Kind,
                Order = action.Order,
                Succeeded = true,
                Message = "ok"
            });
        }
    }
}
