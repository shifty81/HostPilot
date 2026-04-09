using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.UI.DynamicForms.ViewModels;

namespace HostPilot.UI.DynamicForms.FieldViewModels;

public sealed class SelectFieldViewModel : DynamicFieldViewModel
{
    public SelectFieldViewModel(ManifestFieldDefinition definition, object? initialValue) : base(definition, initialValue) { }
    public IReadOnlyList<ManifestFieldOptionDefinition> Options => Definition.Options;
}
