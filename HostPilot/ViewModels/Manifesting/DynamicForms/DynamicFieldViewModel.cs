using System.Collections.ObjectModel;
using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.UI.DynamicForms.ViewModels;

public abstract class DynamicFieldViewModel : ObservableObject
{
    private object? _value;
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private bool _isRequired;

    protected DynamicFieldViewModel(ManifestFieldDefinition definition, object? initialValue)
    {
        Definition = definition;
        _value = initialValue;
    }

    public ManifestFieldDefinition Definition { get; }
    public string Label => Definition.Label;
    public string? Description => Definition.Description;

    public object? Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public bool IsRequired
    {
        get => _isRequired;
        set => SetProperty(ref _isRequired, value);
    }

    public ObservableCollection<ManifestValidationIssue> ValidationIssues { get; } = [];
    public event EventHandler? ValueChanged;
}
