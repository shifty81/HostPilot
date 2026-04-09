namespace HostPilot.UI.DynamicForms.ViewModels;

public sealed class StartupPreviewViewModel : ObservableObject
{
    private string _commandPreview = string.Empty;

    public string CommandPreview
    {
        get => _commandPreview;
        set => SetProperty(ref _commandPreview, value);
    }
}
