using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using HostPilot.Commands;
using HostPilot.Core.Models.DynamicForms;
using HostPilot.Core.Services.DynamicForms;
using HostPilot.Services.DynamicForms;

namespace HostPilot.ViewModels.DynamicForms;

public sealed class DynamicFormViewModel : ViewModelBase
{
    private readonly DynamicFormRuleEngine _ruleEngine = new();
    private readonly DynamicFormValidationService _validationService = new();

    private FormTabViewModel? _selectedTab;
    private bool _hasErrors;
    private string _exportedJson = string.Empty;

    public ObservableCollection<FieldViewModel> Fields { get; } = new();
    public ObservableCollection<FormTabViewModel> Tabs { get; } = new();

    public IReadOnlyList<ManifestRuleDto> Rules { get; private set; } = new List<ManifestRuleDto>();

    public FormTabViewModel? SelectedTab
    {
        get => _selectedTab;
        set => SetProperty(ref _selectedTab, value);
    }

    public bool HasErrors
    {
        get => _hasErrors;
        set => SetProperty(ref _hasErrors, value);
    }

    public string ExportedJson
    {
        get => _exportedJson;
        set => SetProperty(ref _exportedJson, value);
    }

    public ICommand ValidateCommand { get; }
    public ICommand ExportJsonCommand { get; }
    public ICommand ResetToDefaultsCommand { get; }

    public DynamicFormViewModel()
    {
        ValidateCommand = new RelayCommand(_ => ValidateAndRefresh());
        ExportJsonCommand = new RelayCommand(_ => ExportToJson());
        ResetToDefaultsCommand = new RelayCommand(_ => ResetToDefaults());
    }

    public void Load(ManifestDocument manifest)
    {
        foreach (var existing in Fields)
        {
            existing.PropertyChanged -= OnFieldPropertyChanged;
        }

        Fields.Clear();
        Tabs.Clear();

        Rules = manifest.Rules;

        foreach (var fieldDto in manifest.Fields)
        {
            var field = FieldViewModel.FromDto(fieldDto);
            field.PropertyChanged += OnFieldPropertyChanged;
            Fields.Add(field);
        }

        foreach (var tabDto in manifest.Tabs.OrderBy(x => x.Order).ThenBy(x => x.Title))
        {
            var tab = new FormTabViewModel
            {
                Key = tabDto.Key,
                Title = tabDto.Title,
                Order = tabDto.Order
            };

            foreach (var field in Fields.Where(x => x.TabKey == tabDto.Key).OrderBy(x => x.Group).ThenBy(x => x.Order))
            {
                tab.Fields.Add(field);
            }

            tab.SortFields();
            Tabs.Add(tab);
        }

        if (Tabs.Count == 0)
        {
            var fallback = new FormTabViewModel
            {
                Key = "general",
                Title = "General",
                Order = 0
            };

            foreach (var field in Fields.OrderBy(x => x.Group).ThenBy(x => x.Order))
            {
                fallback.Fields.Add(field);
            }

            fallback.SortFields();
            Tabs.Add(fallback);
        }

        SelectedTab = Tabs.FirstOrDefault();
        ValidateAndRefresh();
        ExportToJson();
    }

    public Dictionary<string, object?> GetValues()
    {
        return Fields.ToDictionary(x => x.Key, x => x.Value);
    }

    public void ValidateAndRefresh()
    {
        _ruleEngine.ApplyRules(this, Rules);
        _validationService.ValidateAll(this);
    }

    public void ExportToJson()
    {
        var values = GetValues();
        ExportedJson = JsonSerializer.Serialize(values, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public void ResetToDefaults()
    {
        foreach (var field in Fields)
        {
            field.Value = field.DefaultValue;
        }

        ValidateAndRefresh();
        ExportToJson();
    }

    private void OnFieldPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FieldViewModel.Value))
        {
            ValidateAndRefresh();
            ExportToJson();
        }
    }
}
