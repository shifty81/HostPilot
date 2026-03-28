namespace HostPilot.Core.Services.Providers;

public interface ISecretStore
{
    Task<string?> GetSecretAsync(string key, CancellationToken cancellationToken = default);
    Task SetSecretAsync(string key, string value, CancellationToken cancellationToken = default);
    Task RemoveSecretAsync(string key, CancellationToken cancellationToken = default);
}
