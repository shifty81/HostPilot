using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.UI.DynamicForms.ViewModels;

namespace HostPilot.UI.DynamicForms.Rendering;

public static class DynamicLayoutBuilder
{
    public static IReadOnlyList<DynamicTabViewModel> Build(ProviderManifest manifest, IReadOnlyDictionary<string, object?> state)
    {
        var sectionsByTab = manifest.Sections
            .OrderBy(x => x.Order)
            .GroupBy(x => x.TabId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

        var fieldsBySection = manifest.Fields
            .OrderBy(x => x.Order)
            .GroupBy(x => x.SectionId, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

        var tabs = new List<DynamicTabViewModel>();

        foreach (var tab in manifest.Tabs.OrderBy(x => x.Order))
        {
            var tabVm = new DynamicTabViewModel { Definition = tab };
            if (sectionsByTab.TryGetValue(tab.Id, out var sections))
            {
                foreach (var section in sections)
                {
                    var sectionVm = new DynamicSectionViewModel
                    {
                        Definition = section,
                        IsExpanded = section.InitiallyExpanded
                    };

                    if (fieldsBySection.TryGetValue(section.Id, out var fields))
                    {
                        foreach (var field in fields)
                        {
                            state.TryGetValue(field.Key, out var value);
                            sectionVm.Fields.Add(DynamicFieldViewModelFactory.Create(field, value));
                        }
                    }

                    tabVm.Sections.Add(sectionVm);
                }
            }

            tabs.Add(tabVm);
        }

        return tabs;
    }
}
