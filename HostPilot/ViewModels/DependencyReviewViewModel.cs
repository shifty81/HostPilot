using System.Collections.ObjectModel;
using HostPilot.Core.Models.Mods;

namespace HostPilot.ViewModels;

public sealed class DependencyReviewViewModel
{
    public ObservableCollection<DependencyReviewItem> Items { get; } = new();
    public string Summary => $"{Items.Count(x => x.IsSelected && !x.IsAlreadyInstalled)} dependency installs selected";

    public void Load(IEnumerable<DependencyReviewItem> items)
    {
        Items.Clear();
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }
}
