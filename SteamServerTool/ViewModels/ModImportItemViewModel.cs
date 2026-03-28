using System.ComponentModel;
using System.Runtime.CompilerServices;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.ViewModels;

public class ModImportItemViewModel : INotifyPropertyChanged
{
    private bool _isSelected = true;

    public string DisplayName { get; set; } = "";
    public string SourcePath { get; set; } = "";
    public string DestinationPath { get; set; } = "";
    public ModInstallAction Action { get; set; }
    public string WarningSummary { get; set; } = "";

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
