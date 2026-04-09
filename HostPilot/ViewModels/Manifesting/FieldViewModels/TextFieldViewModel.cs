using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.UI.DynamicForms.ViewModels;

namespace HostPilot.UI.DynamicForms.FieldViewModels;

public sealed class TextFieldViewModel : DynamicFieldViewModel
{
    public TextFieldViewModel(ManifestFieldDefinition definition, object? initialValue) : base(definition, initialValue) { }
}
