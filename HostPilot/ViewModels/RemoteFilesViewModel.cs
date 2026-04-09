using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Remote.Files;

namespace HostPilot.ViewModels;

public sealed class RemoteFilesViewModel : INotifyPropertyChanged
{
    private readonly RemoteFileBrowserService _service;
    private string _currentPath = string.Empty;

    public RemoteFilesViewModel(RemoteFileBrowserService service)
    {
        _service = service;
    }

    public ObservableCollection<RemoteFileEntry> Entries { get; } = new();

    public string CurrentPath
    {
        get => _currentPath;
        private set
        {
            if (_currentPath != value)
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }
    }

    public string SelectedNodeId { get; set; } = string.Empty;
    public string SelectedRootAlias { get; set; } = "server-root";
    public string SelectedRootPath { get; set; } = @"C:\Servers";

    public async Task RefreshAsync(string requestedPath, CancellationToken cancellationToken = default)
    {
        var listing = await _service.ListAsync(SelectedNodeId, SelectedRootAlias, SelectedRootPath, requestedPath, cancellationToken);
        Entries.Clear();
        foreach (var entry in listing.Entries)
        {
            Entries.Add(entry);
        }

        CurrentPath = listing.CurrentPath;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
