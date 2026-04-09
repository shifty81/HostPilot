using System.Collections.ObjectModel;
using HostPilot.Remote.Contracts.Models;

namespace HostPilot.ViewModels.RemoteNodes;

public sealed class RemoteUpdatesViewModel
{
    public ObservableCollection<RemoteUpdateProgress> ProgressItems { get; } = new();
    public string SelectedChannel { get; set; } = "stable";
}
