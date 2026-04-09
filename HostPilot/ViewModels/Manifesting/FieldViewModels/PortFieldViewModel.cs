using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.UI.DynamicForms.ViewModels;

namespace HostPilot.UI.DynamicForms.FieldViewModels;

public sealed class PortFieldViewModel : DynamicFieldViewModel
{
    public PortFieldViewModel(ManifestFieldDefinition definition, object? initialValue) : base(definition, initialValue) { }
}
