using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Core.Automation.Contracts;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Core.Automation.Services;

public sealed class AutomationEngine
{
    private readonly IAutomationRuleStore _ruleStore;
    private readonly IAutomationActionDispatcher _actionDispatcher;
    private readonly AutomationPolicyGuard _policyGuard;
    private readonly ConcurrentDictionary<Guid, DateTimeOffset> _lastExecutionUtc = new();

    public AutomationEngine(
        IAutomationRuleStore ruleStore,
        IAutomationActionDispatcher actionDispatcher,
        AutomationPolicyGuard policyGuard)
    {
        _ruleStore = ruleStore;
        _actionDispatcher = actionDispatcher;
        _policyGuard = policyGuard;
    }

    public async Task HandleEventAsync(RuntimeEvent runtimeEvent, CancellationToken cancellationToken = default)
    {
        var rules = await _ruleStore.GetRulesAsync(cancellationToken);
        var matchingRules = rules
            .Where(r => r.IsEnabled)
            .Where(r => string.Equals(r.Trigger.EventType, runtimeEvent.EventType, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.IsNullOrWhiteSpace(r.TargetServerId) || r.TargetServerId == runtimeEvent.ServerId)
            .OrderBy(r => r.Name)
            .ToList();

        foreach (var rule in matchingRules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!PassesCooldown(rule))
            {
                continue;
            }

            var decision = _policyGuard.Evaluate(rule, runtimeEvent);
            if (!decision.Allowed)
            {
                await _ruleStore.AppendExecutionAsync(new AutomationExecutionRecord
                {
                    RuleId = rule.Id,
                    EventId = runtimeEvent.Id,
                    ServerId = runtimeEvent.ServerId,
                    StartedUtc = DateTimeOffset.UtcNow,
                    FinishedUtc = DateTimeOffset.UtcNow,
                    Result = AutomationExecutionResults.Rejected,
                    Summary = decision.Message
                }, cancellationToken);
                continue;
            }

            var record = new AutomationExecutionRecord
            {
                RuleId = rule.Id,
                EventId = runtimeEvent.Id,
                ServerId = runtimeEvent.ServerId,
                StartedUtc = DateTimeOffset.UtcNow,
                Result = AutomationExecutionResults.Running,
                Summary = $"Executing rule '{rule.Name}'."
            };

            foreach (var action in rule.Actions.OrderBy(a => a.Order))
            {
                if (action.DelaySeconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(action.DelaySeconds), cancellationToken);
                }

                var result = await _actionDispatcher.DispatchAsync(rule, action, runtimeEvent, cancellationToken);
                record.ActionResults.Add(result);

                if (!result.Succeeded && !action.ContinueOnFailure)
                {
                    record.Result = AutomationExecutionResults.Failed;
                    record.Summary = result.Message;
                    record.FinishedUtc = DateTimeOffset.UtcNow;
                    await _ruleStore.AppendExecutionAsync(record, cancellationToken);
                    _lastExecutionUtc[rule.Id] = DateTimeOffset.UtcNow;
                    goto NextRule;
                }
            }

            record.Result = AutomationExecutionResults.Succeeded;
            record.Summary = $"Rule '{rule.Name}' completed.";
            record.FinishedUtc = DateTimeOffset.UtcNow;
            await _ruleStore.AppendExecutionAsync(record, cancellationToken);
            _lastExecutionUtc[rule.Id] = DateTimeOffset.UtcNow;

        NextRule:;
        }
    }

    private bool PassesCooldown(AutomationRule rule)
    {
        if (rule.Cooldown is null)
        {
            return true;
        }

        if (!_lastExecutionUtc.TryGetValue(rule.Id, out var lastRun))
        {
            return true;
        }

        return DateTimeOffset.UtcNow - lastRun >= rule.Cooldown.Value;
    }
}
