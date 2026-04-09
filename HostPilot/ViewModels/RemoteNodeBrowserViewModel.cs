using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HostPilot.Remote.Contracts.Models;

namespace HostPilot.ViewModels;

public sealed class RemoteNodeBrowserViewModel : INotifyPropertyChanged
{
    private RemoteNodeIdentity? _selectedNode;

    public ObservableCollection<RemoteNodeIdentity> Nodes { get; } = new();

    public RemoteNodeIdentity? SelectedNode
    {
        get => _selectedNode;
        set
        {
            if (!ReferenceEquals(_selectedNode, value))
            {
                _selectedNode = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
