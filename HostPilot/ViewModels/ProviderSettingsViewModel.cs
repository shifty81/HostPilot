using System.ComponentModel;
using System.Runtime.CompilerServices;
using HostPilot.Core.Models.Providers;
using HostPilot.Core.Services.Providers;

namespace HostPilot.ViewModels;

public sealed class ProviderSettingsViewModel : INotifyPropertyChanged
{
    private readonly IProviderSettingsStore _settingsStore;
    private readonly ISecretStore _secretStore;
    private ProviderSettings _settings = new();
    private string _curseForgeApiKey = string.Empty;
    private string _status = string.Empty;

    public ProviderSettingsViewModel(IProviderSettingsStore settingsStore, ISecretStore secretStore)
    {
        _settingsStore = settingsStore;
        _secretStore = secretStore;
    }

    public ProviderSettings Settings
    {
        get => _settings;
        private set
        {
            _settings = value;
            OnPropertyChanged();
        }
    }

    public string CurseForgeApiKey
    {
        get => _curseForgeApiKey;
        set
        {
            if (_curseForgeApiKey == value) return;
            _curseForgeApiKey = value;
            OnPropertyChanged();
        }
    }

    public string Status
    {
        get => _status;
        private set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Settings = await _settingsStore.LoadAsync(cancellationToken).ConfigureAwait(false);
        CurseForgeApiKey = await _secretStore.GetSecretAsync(Settings.CurseForgeApiKeyName, cancellationToken).ConfigureAwait(false) ?? string.Empty;
        Status = "Provider settings loaded.";
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _settingsStore.SaveAsync(Settings, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(CurseForgeApiKey))
        {
            await _secretStore.SetSecretAsync(Settings.CurseForgeApiKeyName, CurseForgeApiKey, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await _secretStore.RemoveSecretAsync(Settings.CurseForgeApiKeyName, cancellationToken).ConfigureAwait(false);
        }

        Status = "Provider settings saved.";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
