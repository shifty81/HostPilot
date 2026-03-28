using HostPilot.Core.Models.Providers;

namespace HostPilot.Core.Services.Providers;

public interface IProviderSettingsStore
{
    Task<ProviderSettings> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(ProviderSettings settings, CancellationToken cancellationToken = default);
}
