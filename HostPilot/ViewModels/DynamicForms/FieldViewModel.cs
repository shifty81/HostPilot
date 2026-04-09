using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using HostPilot.Commands;
using HostPilot.Core.Models.DynamicForms;
using Microsoft.Win32;

namespace HostPilot.ViewModels.DynamicForms;

public sealed class FieldViewModel : ViewModelBase
{
    private object? _value;
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private string? _validationError;

    public string Key { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Group { get; init; } = "General";
    public string TabKey { get; init; } = "general";
    public int Order { get; init; }
    public DynamicFieldType Type { get; init; }
    public bool IsRequired { get; init; }
    public object? DefaultValue { get; init; }
    public object? Min { get; init; }
    public object? Max { get; init; }
    public string? Placeholder { get; init; }
    public string? BrowseMode { get; init; }
    public string? ValidationRegex { get; init; }
    public string? FileFilter { get; init; }

    public ObservableCollection<FieldOption> Options { get; } = new();

    public object? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public string? ValidationError
    {
        get => _validationError;
        set
        {
            if (SetProperty(ref _validationError, value))
            {
                RaisePropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ValidationError);

    public ICommand BrowseCommand { get; }

    public FieldViewModel()
    {
        BrowseCommand = new RelayCommand(_ => Browse(), _ => Type is DynamicFieldType.FilePath or DynamicFieldType.FolderPath);
    }

    private void Browse()
    {
        if (Type == DynamicFieldType.FilePath)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                Title = $"Select {Label}",
                Filter = string.IsNullOrWhiteSpace(FileFilter) ? "All Files (*.*)|*.*" : FileFilter
            };

            if (dialog.ShowDialog() == true)
            {
                Value = dialog.FileName;
            }
        }
        else if (Type == DynamicFieldType.FolderPath)
        {
            var dialog = new OpenFolderDialog
            {
                Title = $"Select {Label}"
            };

            if (dialog.ShowDialog() == true)
            {
                Value = dialog.FolderName;
            }
        }
    }

    public string? Validate()
    {
        var text = Value?.ToString() ?? string.Empty;

        if (IsRequired && string.IsNullOrWhiteSpace(text))
        {
            return $"{Label} is required.";
        }

        if (!string.IsNullOrWhiteSpace(ValidationRegex) && !string.IsNullOrWhiteSpace(text))
        {
            if (!Regex.IsMatch(text, ValidationRegex))
            {
                return $"{Label} format is invalid.";
            }
        }

        if (Type == DynamicFieldType.Number && !string.IsNullOrWhiteSpace(text))
        {
            if (!int.TryParse(text, out var parsed))
            {
                return $"{Label} must be a valid integer.";
            }

            if (Min != null && parsed < Convert.ToInt32(Min))
            {
                return $"{Label} must be at least {Min}.";
            }

            if (Max != null && parsed > Convert.ToInt32(Max))
            {
                return $"{Label} must be at most {Max}.";
            }
        }

        if (Type == DynamicFieldType.Decimal && !string.IsNullOrWhiteSpace(text))
        {
            if (!decimal.TryParse(text, out var parsed))
            {
                return $"{Label} must be a valid decimal.";
            }

            if (Min != null && parsed < Convert.ToDecimal(Min))
            {
                return $"{Label} must be at least {Min}.";
            }

            if (Max != null && parsed > Convert.ToDecimal(Max))
            {
                return $"{Label} must be at most {Max}.";
            }
        }

        return null;
    }

    public static FieldViewModel FromDto(ManifestFieldDto dto)
    {
        var type = Enum.TryParse<DynamicFieldType>(dto.Type, true, out var parsedType)
            ? parsedType
            : DynamicFieldType.Text;

        var field = new FieldViewModel
        {
            Key = dto.Key,
            Label = dto.Label,
            Description = dto.Description,
            Type = type,
            TabKey = dto.TabKey,
            Group = dto.Group,
            Order = dto.Order,
            IsRequired = dto.IsRequired,
            DefaultValue = dto.DefaultValue,
            Value = dto.DefaultValue,
            Min = dto.Min,
            Max = dto.Max,
            Placeholder = dto.Placeholder,
            BrowseMode = dto.BrowseMode,
            ValidationRegex = dto.ValidationRegex,
            FileFilter = dto.FileFilter
        };

        if (dto.Options != null)
        {
            foreach (var option in dto.Options)
            {
                field.Options.Add(option);
            }
        }

        return field;
    }
}
