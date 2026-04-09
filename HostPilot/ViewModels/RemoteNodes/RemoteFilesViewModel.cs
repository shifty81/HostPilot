using System.Collections.ObjectModel;
using HostPilot.Remote.Contracts.Models;

namespace HostPilot.ViewModels.RemoteNodes;

public sealed class RemoteFilesViewModel
{
    public ObservableCollection<RemoteFileEntry> Entries { get; } = new();
    public ObservableCollection<RemoteTransferProgress> Transfers { get; } = new();
    public string CurrentPath { get; set; } = "/";
}
