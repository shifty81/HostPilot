using System;
using System.Collections.Generic;
using System.Linq;
using HostPilot.Core.Models.DynamicForms;
using HostPilot.ViewModels.DynamicForms;

namespace HostPilot.Services.DynamicForms;

public sealed class DynamicFormRuleEngine
{
    public void ApplyRules(DynamicFormViewModel form, IReadOnlyList<ManifestRuleDto> rules)
    {
        foreach (var field in form.Fields)
        {
            field.IsVisible = true;
            field.IsEnabled = true;
        }

        foreach (var rule in rules)
        {
            var source = form.Fields.FirstOrDefault(x => x.Key == rule.SourceKey);
            var target = form.Fields.FirstOrDefault(x => x.Key == rule.TargetKey);

            if (source == null || target == null)
            {
                continue;
            }

            var sourceValue = source.Value?.ToString() ?? string.Empty;
            var expected = rule.Equals ?? string.Empty;
            var matches = string.Equals(sourceValue, expected, StringComparison.OrdinalIgnoreCase);

            switch (rule.Action.ToLowerInvariant())
            {
                case "show":
                    if (matches)
                    {
                        target.IsVisible = true;
                    }
                    break;

                case "hide":
                    if (matches)
                    {
                        target.IsVisible = false;
                    }
                    break;

                case "enable":
                    if (matches)
                    {
                        target.IsEnabled = true;
                    }
                    break;

                case "disable":
                    if (matches)
                    {
                        target.IsEnabled = false;
                    }
                    break;
            }
        }
    }
}
