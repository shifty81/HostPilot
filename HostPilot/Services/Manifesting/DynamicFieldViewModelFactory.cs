using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Enums;
using HostPilot.UI.DynamicForms.FieldViewModels;
using HostPilot.UI.DynamicForms.ViewModels;

namespace HostPilot.UI.DynamicForms.Rendering;

public static class DynamicFieldViewModelFactory
{
    public static DynamicFieldViewModel Create(ManifestFieldDefinition field, object? initialValue)
    {
        return field.Type switch
        {
            ManifestFieldType.Text => new TextFieldViewModel(field, initialValue),
            ManifestFieldType.TextArea => new TextAreaFieldViewModel(field, initialValue),
            ManifestFieldType.Password => new PasswordFieldViewModel(field, initialValue),
            ManifestFieldType.Number => new NumberFieldViewModel(field, initialValue),
            ManifestFieldType.Checkbox => new CheckboxFieldViewModel(field, initialValue),
            ManifestFieldType.Select => new SelectFieldViewModel(field, initialValue),
            ManifestFieldType.Port => new PortFieldViewModel(field, initialValue),
            _ => new TextFieldViewModel(field, initialValue)
        };
    }
}
