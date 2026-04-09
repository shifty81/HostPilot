using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HostPilot.Core.Automation.Models;

namespace HostPilot.Automation;

public sealed class AutomationRulesViewModel : INotifyPropertyChanged
{
    private AutomationRule? _selectedRule;

    public ObservableCollection<AutomationRule> Rules { get; } = new();

    public AutomationRule? SelectedRule
    {
        get => _selectedRule;
        set
        {
            if (_selectedRule == value)
            {
                return;
            }

            _selectedRule = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
