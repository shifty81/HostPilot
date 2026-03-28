using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using HostPilot.Commands;
using HostPilot.Core.Models;
using HostPilot.Core.Models.Mods;
using HostPilot.Core.Services.Mods;

namespace HostPilot.ViewModels;

public sealed class ModsTabViewModel : INotifyPropertyChanged
{
    private readonly ModCatalogService _catalogService;
    private readonly TemplateModProfileResolver _profileResolver;
    private readonly IModInstallCoordinator _installCoordinator;

    private ServerTemplate? _selectedTemplate;
    private ServerConfig? _selectedServerConfig;
    private ModSupportProfile? _activeProfile;
    private string? _searchText;
    private bool _isBusy;
    private ModCatalogItem? _selectedCatalogItem;
    private InstalledModEntry? _selectedInstalledMod;
    private string _profileSummary = "Select a server template to load mod support.";

    public ModsTabViewModel(
        ModCatalogService catalogService,
        TemplateModProfileResolver profileResolver,
        IModInstallCoordinator installCoordinator)
    {
        _catalogService = catalogService;
        _profileResolver = profileResolver;
        _installCoordinator = installCoordinator;

        SearchCommand = new RelayCommand(async _ => await SearchAsync().ConfigureAwait(false), _ => _selectedTemplate is not null && !IsBusy);
        InstallSelectedCommand = new RelayCommand(async _ => await InstallSelectedAsync().ConfigureAwait(false), _ => SelectedCatalogItem is not null && _selectedServerConfig is not null && !IsBusy);
        RefreshInstalledCommand = new RelayCommand(async _ => await RefreshInstalledAsync().ConfigureAwait(false), _ => _selectedServerConfig is not null && _activeProfile is not null && !IsBusy);
    }

    public ObservableCollection<ModCatalogItem> CatalogResults { get; } = new();
    public ObservableCollection<InstalledModEntry> InstalledMods { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand InstallSelectedCommand { get; }
    public ICommand RefreshInstalledCommand { get; }

    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value) return;
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    public string ProfileSummary
    {
        get => _profileSummary;
        private set
        {
            if (_profileSummary == value) return;
            _profileSummary = value;
            OnPropertyChanged();
        }
    }

    public ModCatalogItem? SelectedCatalogItem
    {
        get => _selectedCatalogItem;
        set
        {
            if (_selectedCatalogItem == value) return;
            _selectedCatalogItem = value;
            OnPropertyChanged();
            RaiseCommandStates();
        }
    }

    public InstalledModEntry? SelectedInstalledMod
    {
        get => _selectedInstalledMod;
        set
        {
            if (_selectedInstalledMod == value) return;
            _selectedInstalledMod = value;
            OnPropertyChanged();
        }
    }

    public bool SupportsBrowser => _activeProfile?.SupportsBrowser == true;
    public bool SupportsDependencies => _activeProfile?.SupportsDependencies == true;
    public string InstallRoot => _activeProfile?.InstallRootRelativePath ?? "mods";

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<string>? StatusMessage;

    public async Task SetContextAsync(ServerConfig serverConfig, ServerTemplate? template)
    {
        _selectedServerConfig = serverConfig;
        _selectedTemplate = template;
        _activeProfile = template is null ? null : _profileResolver.Resolve(template);

        ProfileSummary = _activeProfile is null
            ? "No template profile resolved. Local intake only."
            : $"Primary provider: {_activeProfile.PrimaryProvider} • Install root: {_activeProfile.InstallRootRelativePath} • Browser: {(_activeProfile.SupportsBrowser ? "Yes" : "No")} • Dependencies: {(_activeProfile.SupportsDependencies ? "Yes" : "No")}";

        OnPropertyChanged(nameof(SupportsBrowser));
        OnPropertyChanged(nameof(SupportsDependencies));
        OnPropertyChanged(nameof(InstallRoot));

        CatalogResults.Clear();
        await RefreshInstalledAsync().ConfigureAwait(false);
        RaiseCommandStates();
    }

    public async Task SearchAsync()
    {
        if (_selectedTemplate is null)
            return;

        var profile = _activeProfile ?? _profileResolver.Resolve(_selectedTemplate);
        if (!profile.SupportsBrowser)
        {
            StatusMessage?.Invoke(this, "This server template uses local/manual mod intake only.");
            return;
        }

        IsBusy = true;
        try
        {
            CatalogResults.Clear();
            var query = new ModBrowserQuery
            {
                SearchText = SearchText,
                GameVersion = profile.RequiredGameVersion,
                LoaderType = profile.RequiredLoader,
            };

            var results = await _catalogService.SearchAsync(profile, query).ConfigureAwait(false);
            foreach (var item in results)
                CatalogResults.Add(item);

            StatusMessage?.Invoke(this, $"Loaded {CatalogResults.Count} mod result(s).");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RefreshInstalledAsync()
    {
        if (_selectedServerConfig is null || _activeProfile is null)
            return;

        IsBusy = true;
        try
        {
            InstalledMods.Clear();
            var installed = await _installCoordinator.GetInstalledAsync(_selectedServerConfig, _activeProfile).ConfigureAwait(false);
            foreach (var item in installed)
                InstalledMods.Add(item);

            StatusMessage?.Invoke(this, $"Found {InstalledMods.Count} installed/configured mod item(s).");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task InstallSelectedAsync()
    {
        if (_selectedServerConfig is null || _activeProfile is null || SelectedCatalogItem is null)
            return;

        IsBusy = true;
        try
        {
            var installed = await _installCoordinator.InstallCatalogItemAsync(_selectedServerConfig, _activeProfile, SelectedCatalogItem).ConfigureAwait(false);
            InstalledMods.Add(installed);
            StatusMessage?.Invoke(this, $"Installed {installed.Name} to {InstallRoot}.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RaiseCommandStates()
    {
        if (SearchCommand is RelayCommand search) search.RaiseCanExecuteChanged();
        if (InstallSelectedCommand is RelayCommand install) install.RaiseCanExecuteChanged();
        if (RefreshInstalledCommand is RelayCommand refresh) refresh.RaiseCanExecuteChanged();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
