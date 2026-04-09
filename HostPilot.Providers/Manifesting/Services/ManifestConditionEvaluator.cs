using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Enums;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ManifestConditionEvaluator : IManifestConditionEvaluator
{
    public bool Evaluate(ManifestConditionGroup? group, ManifestEvaluationContext context)
    {
        if (group is null)
        {
            return true;
        }

        var childResults = new List<bool>();
        childResults.AddRange(group.Conditions.Select(condition => EvaluateCondition(condition, context.State)));
        childResults.AddRange(group.Groups.Select(child => Evaluate(child, context)));

        if (childResults.Count == 0)
        {
            return true;
        }

        return string.Equals(group.Logic, "Or", StringComparison.OrdinalIgnoreCase)
            ? childResults.Any(x => x)
            : childResults.All(x => x);
    }

    private static bool EvaluateCondition(ManifestConditionDefinition condition, IReadOnlyDictionary<string, object?> state)
    {
        state.TryGetValue(condition.Path, out var actual);
        return condition.Operator switch
        {
            ManifestConditionOperator.Equals => Equals(actual, condition.Value),
            ManifestConditionOperator.NotEquals => !Equals(actual, condition.Value),
            ManifestConditionOperator.Exists => state.ContainsKey(condition.Path),
            ManifestConditionOperator.IsEmpty => actual is null || string.IsNullOrWhiteSpace(actual.ToString()),
            ManifestConditionOperator.NotEmpty => actual is not null && !string.IsNullOrWhiteSpace(actual.ToString()),
            ManifestConditionOperator.GreaterThan => CompareAsDouble(actual, condition.Value) > 0,
            ManifestConditionOperator.GreaterThanOrEqual => CompareAsDouble(actual, condition.Value) >= 0,
            ManifestConditionOperator.LessThan => CompareAsDouble(actual, condition.Value) < 0,
            ManifestConditionOperator.LessThanOrEqual => CompareAsDouble(actual, condition.Value) <= 0,
            ManifestConditionOperator.Contains => (actual?.ToString() ?? string.Empty).Contains(condition.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase),
            ManifestConditionOperator.In => In(actual, condition.Value),
            ManifestConditionOperator.NotIn => !In(actual, condition.Value),
            _ => true
        };
    }

    private static int CompareAsDouble(object? left, object? right)
    {
        var lhs = Convert.ToDouble(left ?? 0d);
        var rhs = Convert.ToDouble(right ?? 0d);
        return lhs.CompareTo(rhs);
    }

    private static bool In(object? actual, object? value)
    {
        if (value is IEnumerable<object> objects)
        {
            return objects.Any(x => Equals(actual, x));
        }

        if (value is IEnumerable<string> strings)
        {
            return strings.Any(x => string.Equals(actual?.ToString(), x, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }
}
