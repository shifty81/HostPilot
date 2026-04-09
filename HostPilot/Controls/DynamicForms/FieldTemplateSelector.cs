using System.Windows;
using System.Windows.Controls;
using HostPilot.Core.Models.DynamicForms;
using HostPilot.ViewModels.DynamicForms;

namespace HostPilot.Controls.DynamicForms;

public sealed class FieldTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? MultilineTemplate { get; set; }
    public DataTemplate? NumberTemplate { get; set; }
    public DataTemplate? CheckboxTemplate { get; set; }
    public DataTemplate? DropdownTemplate { get; set; }
    public DataTemplate? PasswordTemplate { get; set; }
    public DataTemplate? FileTemplate { get; set; }
    public DataTemplate? FolderTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item is not FieldViewModel field)
        {
            return base.SelectTemplate(item, container);
        }

        return field.Type switch
        {
            DynamicFieldType.MultilineText => MultilineTemplate,
            DynamicFieldType.Number => NumberTemplate,
            DynamicFieldType.Decimal => NumberTemplate,
            DynamicFieldType.Checkbox => CheckboxTemplate,
            DynamicFieldType.Dropdown => DropdownTemplate,
            DynamicFieldType.Password => PasswordTemplate,
            DynamicFieldType.FilePath => FileTemplate,
            DynamicFieldType.FolderPath => FolderTemplate,
            _ => TextTemplate
        };
    }
}
