using System.Collections.ObjectModel;
using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.UI.DynamicForms.ViewModels;

public sealed class DynamicTabViewModel : ObservableObject
{
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private int _errorCount;

    public required ManifestTabDefinition Definition { get; init; }
    public ObservableCollection<DynamicSectionViewModel> Sections { get; } = [];
    public string Title => Definition.Title;

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

    public int ErrorCount
    {
        get => _errorCount;
        set => SetProperty(ref _errorCount, value);
    }
}
