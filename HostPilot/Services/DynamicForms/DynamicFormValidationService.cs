using System.Linq;
using HostPilot.ViewModels.DynamicForms;

namespace HostPilot.Services.DynamicForms;

public sealed class DynamicFormValidationService
{
    public void ValidateAll(DynamicFormViewModel form)
    {
        foreach (var field in form.Fields)
        {
            field.ValidationError = field.IsVisible ? field.Validate() : null;
        }

        form.HasErrors = form.Fields.Any(x => x.IsVisible && x.HasError);
    }
}
