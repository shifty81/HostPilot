using System.Collections.ObjectModel;
using System.Text;
using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;
using HostPilot.Providers.Manifesting.Services;
using HostPilot.UI.DynamicForms.Rendering;

namespace HostPilot.UI.DynamicForms.ViewModels;

public sealed class DynamicConfigPageViewModel : ObservableObject
{
    private readonly IManifestDefaultResolver _defaultResolver;
    private readonly IManifestConditionEvaluator _conditionEvaluator;
    private readonly IManifestValidationEngine _validationEngine;
    private readonly IManifestOutputMapper _outputMapper;
    private bool _isAdvancedMode;

    public DynamicConfigPageViewModel(
        IManifestDefaultResolver defaultResolver,
        IManifestConditionEvaluator conditionEvaluator,
        IManifestValidationEngine validationEngine,
        IManifestOutputMapper outputMapper)
    {
        _defaultResolver = defaultResolver;
        _conditionEvaluator = conditionEvaluator;
        _validationEngine = validationEngine;
        _outputMapper = outputMapper;
    }

    public ProviderManifest? Manifest { get; private set; }
    public ObservableCollection<DynamicTabViewModel> Tabs { get; } = [];
    public Dictionary<string, object?> State { get; } = new(StringComparer.OrdinalIgnoreCase);
    public ValidationSummaryViewModel ValidationSummary { get; } = new();
    public StartupPreviewViewModel StartupPreview { get; } = new();

    public bool IsAdvancedMode
    {
        get => _isAdvancedMode;
        set
        {
            if (SetProperty(ref _isAdvancedMode, value))
            {
                Reevaluate();
            }
        }
    }

    public void Load(ProviderManifest manifest, string? presetId = null)
    {
        Manifest = manifest;
        Tabs.Clear();
        State.Clear();

        foreach (var pair in _defaultResolver.BuildDefaultState(manifest, presetId))
        {
            State[pair.Key] = pair.Value;
        }

        foreach (var tab in DynamicLayoutBuilder.Build(manifest, State))
        {
            foreach (var section in tab.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.ValueChanged += OnFieldValueChanged;
                }
            }
            Tabs.Add(tab);
        }

        Reevaluate();
    }

    private void OnFieldValueChanged(object? sender, EventArgs e)
    {
        if (sender is not DynamicFieldViewModel field || Manifest is null)
        {
            return;
        }

        State[field.Definition.Key] = field.Value;
        Reevaluate();
    }

    private void Reevaluate()
    {
        if (Manifest is null)
        {
            return;
        }

        var context = new ManifestEvaluationContext
        {
            Manifest = Manifest,
            State = State,
            IsAdvancedMode = IsAdvancedMode
        };

        foreach (var tab in Tabs)
        {
            tab.IsVisible = _conditionEvaluator.Evaluate(tab.Definition.VisibleWhen, context) && (!tab.Definition.Advanced || IsAdvancedMode);
            tab.IsEnabled = _conditionEvaluator.Evaluate(tab.Definition.EnabledWhen, context);

            foreach (var section in tab.Sections)
            {
                section.IsVisible = _conditionEvaluator.Evaluate(section.Definition.VisibleWhen, context) && (!section.Definition.Advanced || IsAdvancedMode);
                section.IsEnabled = _conditionEvaluator.Evaluate(section.Definition.EnabledWhen, context);

                foreach (var field in section.Fields)
                {
                    field.IsVisible = _conditionEvaluator.Evaluate(field.Definition.VisibleWhen, context) && (!field.Definition.Advanced || IsAdvancedMode);
                    field.IsEnabled = _conditionEvaluator.Evaluate(field.Definition.EnabledWhen, context);
                    field.IsRequired = field.Definition.Required || _conditionEvaluator.Evaluate(field.Definition.RequiredWhen, context);
                    field.ValidationIssues.Clear();
                }
            }
        }

        var validation = _validationEngine.Validate(Manifest, State, IsAdvancedMode);
        ValidationSummary.Issues.Clear();
        foreach (var issue in validation.Issues)
        {
            ValidationSummary.Issues.Add(issue);
            var fieldVm = Tabs.SelectMany(t => t.Sections).SelectMany(s => s.Fields).FirstOrDefault(f => f.Definition.Id == issue.FieldId);
            fieldVm?.ValidationIssues.Add(issue);
        }

        foreach (var tab in Tabs)
        {
            tab.ErrorCount = tab.Sections.SelectMany(s => s.Fields).Sum(f => f.ValidationIssues.Count);
        }

        var output = _outputMapper.BuildOutput(Manifest, State);
        var builder = new StringBuilder();
        builder.Append(Manifest.Deployment.DefaultExecutable ?? "<executable>");
        if (output.LaunchArguments.Count > 0)
        {
            builder.Append(' ');
            builder.Append(string.Join(' ', output.LaunchArguments));
        }
        StartupPreview.CommandPreview = builder.ToString();
    }
}
