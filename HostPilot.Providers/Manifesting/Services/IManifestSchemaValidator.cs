namespace HostPilot.Providers.Manifesting.Services;

public interface IManifestSchemaValidator
{
    Task ValidateAsync(string json, CancellationToken cancellationToken = default);
}
