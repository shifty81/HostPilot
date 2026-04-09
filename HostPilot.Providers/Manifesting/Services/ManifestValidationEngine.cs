using System.Text.RegularExpressions;
using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Enums;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ManifestValidationEngine : IManifestValidationEngine
{
    private static readonly string[] CrossRuleOperators = ["<=", ">=", "!=", "==", "<", ">"];
    private readonly IManifestConditionEvaluator _conditionEvaluator;

    public ManifestValidationEngine(IManifestConditionEvaluator conditionEvaluator)
    {
        _conditionEvaluator = conditionEvaluator;
    }

    public ManifestValidationResult Validate(ProviderManifest manifest, IReadOnlyDictionary<string, object?> state, bool isAdvancedMode = false)
    {
        var result = new ManifestValidationResult();
        var context = new ManifestEvaluationContext { Manifest = manifest, State = state, IsAdvancedMode = isAdvancedMode };

        foreach (var field in manifest.Fields)
        {
            var visible = _conditionEvaluator.Evaluate(field.VisibleWhen, context) && (!field.Advanced || isAdvancedMode);
            if (!visible)
            {
                continue;
            }

            state.TryGetValue(field.Key, out var value);
            var required = field.Required || _conditionEvaluator.Evaluate(field.RequiredWhen, context);

            if (required && IsEmpty(value))
            {
                result.Issues.Add(Issue("Required", $"{field.Label} is required.", field));
                continue;
            }

            if (value is null)
            {
                continue;
            }

            ValidateBuiltIn(field, value, result);
            ValidateCustom(field, value, state, result);
        }

        foreach (var crossRule in manifest.CrossFieldValidations)
        {
            if (!EvaluateSimpleCrossRule(crossRule.Rule, state))
            {
                result.Issues.Add(new ManifestValidationIssue
                {
                    Code = crossRule.Id,
                    Message = crossRule.Message,
                    Severity = crossRule.Severity
                });
            }
        }

        return result;
    }

    private static void ValidateBuiltIn(ManifestFieldDefinition field, object value, ManifestValidationResult result)
    {
        switch (field.Type)
        {
            case ManifestFieldType.Port:
                if (!int.TryParse(value.ToString(), out var port) || port is < 1 or > 65535)
                {
                    result.Issues.Add(Issue("PortRange", $"{field.Label} must be between 1 and 65535.", field));
                }
                break;
            case ManifestFieldType.Number:
                if (!double.TryParse(value.ToString(), out var number))
                {
                    result.Issues.Add(Issue("Number", $"{field.Label} must be numeric.", field));
                    return;
                }
                if (field.Min.HasValue && number < field.Min.Value)
                {
                    result.Issues.Add(Issue("Min", $"{field.Label} must be at least {field.Min.Value}.", field));
                }
                if (field.Max.HasValue && number > field.Max.Value)
                {
                    result.Issues.Add(Issue("Max", $"{field.Label} must be at most {field.Max.Value}.", field));
                }
                break;
            case ManifestFieldType.Select:
                if (field.Options.Count > 0 && field.Options.All(x => !string.Equals(x.Value, value.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    result.Issues.Add(Issue("AllowedValues", $"{field.Label} must be one of the configured options.", field));
                }
                break;
        }

        if (value is string text)
        {
            if (field.MinLength.HasValue && text.Length < field.MinLength.Value)
            {
                result.Issues.Add(Issue("MinLength", $"{field.Label} is too short.", field));
            }
            if (field.MaxLength.HasValue && text.Length > field.MaxLength.Value)
            {
                result.Issues.Add(Issue("MaxLength", $"{field.Label} is too long.", field));
            }
            if (!string.IsNullOrWhiteSpace(field.RegexPattern) && !Regex.IsMatch(text, field.RegexPattern))
            {
                result.Issues.Add(Issue("Regex", $"{field.Label} format is invalid.", field));
            }
        }
    }

    private static void ValidateCustom(
        ManifestFieldDefinition field,
        object value,
        IReadOnlyDictionary<string, object?> state,
        ManifestValidationResult result)
    {
        foreach (var rule in field.Validation)
        {
            switch (rule.Type)
            {
                case ManifestValidationType.CompareNotEqual:
                    if (!string.IsNullOrWhiteSpace(rule.CompareToKey)
                        && state.TryGetValue(rule.CompareToKey, out var other)
                        && Equals(value, other))
                    {
                        result.Issues.Add(Issue("CompareNotEqual", rule.Message ?? $"{field.Label} must differ.", field));
                    }
                    break;
                case ManifestValidationType.CompareEqual:
                    if (!string.IsNullOrWhiteSpace(rule.CompareToKey)
                        && state.TryGetValue(rule.CompareToKey, out var expected)
                        && !Equals(value, expected))
                    {
                        result.Issues.Add(Issue("CompareEqual", rule.Message ?? $"{field.Label} must match the compared value.", field));
                    }
                    break;
            }
        }
    }

    private static bool EvaluateSimpleCrossRule(string rule, IReadOnlyDictionary<string, object?> state)
    {
        var op = CrossRuleOperators.FirstOrDefault(rule.Contains);
        if (op is null)
        {
            return true;
        }

        var parts = rule.Split(op, 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return true;
        }

        var left = ResolveToken(parts[0], state);
        var right = ResolveToken(parts[1], state);

        return op switch
        {
            "==" => Equals(left, right),
            "!=" => !Equals(left, right),
            ">" => Convert.ToDouble(left) > Convert.ToDouble(right),
            ">=" => Convert.ToDouble(left) >= Convert.ToDouble(right),
            "<" => Convert.ToDouble(left) < Convert.ToDouble(right),
            "<=" => Convert.ToDouble(left) <= Convert.ToDouble(right),
            _ => true
        };
    }

    private static object? ResolveToken(string token, IReadOnlyDictionary<string, object?> state)
    {
        if (state.TryGetValue(token, out var value))
        {
            return value;
        }

        if (bool.TryParse(token, out var b)) return b;
        if (double.TryParse(token, out var d)) return d;
        return token.Trim('"');
    }

    private static bool IsEmpty(object? value) => value is null || string.IsNullOrWhiteSpace(value.ToString());

    private static ManifestValidationIssue Issue(string code, string message, ManifestFieldDefinition field) => new()
    {
        Code = code,
        Message = message,
        Severity = "Error",
        FieldId = field.Id,
        ConfigKey = field.Key
    };
}
