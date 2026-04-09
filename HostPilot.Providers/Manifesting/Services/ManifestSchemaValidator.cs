namespace HostPilot.Providers.Manifesting.Services;

public sealed class ManifestSchemaValidator : IManifestSchemaValidator
{
    public Task ValidateAsync(string json, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("Manifest JSON is empty.");
        }

        return Task.CompletedTask;
    }
}
