using System.Collections.ObjectModel;
using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.UI.DynamicForms.ViewModels;

public sealed class DynamicSectionViewModel : ObservableObject
{
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private bool _isExpanded = true;

    public required ManifestSectionDefinition Definition { get; init; }
    public ObservableCollection<DynamicFieldViewModel> Fields { get; } = [];
    public string Title => Definition.Title;
    public string? Description => Definition.Description;

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

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }
}
