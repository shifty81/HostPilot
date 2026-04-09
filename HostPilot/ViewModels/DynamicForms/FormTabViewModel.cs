using System.Collections.ObjectModel;
using System.Linq;

namespace HostPilot.ViewModels.DynamicForms;

public sealed class FormTabViewModel : ViewModelBase
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public int Order { get; init; }

    public ObservableCollection<FieldViewModel> Fields { get; } = new();

    public void SortFields()
    {
        var ordered = Fields
            .OrderBy(x => x.Group)
            .ThenBy(x => x.Order)
            .ThenBy(x => x.Label)
            .ToList();

        Fields.Clear();

        foreach (var field in ordered)
        {
            Fields.Add(field);
        }
    }
}
