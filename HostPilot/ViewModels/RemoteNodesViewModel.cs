using System.Collections.ObjectModel;
using HostPilot.Contracts;

namespace HostPilot.ViewModels;

public sealed class RemoteNodesViewModel
{
    public ObservableCollection<RemoteNodeDto> Nodes { get; } = new();
    public string FilterText { get; set; } = string.Empty;
}
