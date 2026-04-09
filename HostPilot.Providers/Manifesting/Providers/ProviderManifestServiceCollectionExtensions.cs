using Microsoft.Extensions.DependencyInjection;
using HostPilot.Providers.Manifesting.Services;

namespace HostPilot.Providers.Manifesting.Providers;

public static class ProviderManifestServiceCollectionExtensions
{
    public static IServiceCollection AddProviderManifesting(
        this IServiceCollection services,
        string registryFilePath,
        string manifestRoot)
    {
        services.AddSingleton<IProviderManifestRegistry>(_ => new ProviderManifestRegistry(registryFilePath));
        services.AddSingleton<IManifestSchemaValidator, ManifestSchemaValidator>();
        services.AddSingleton<IManifestInheritanceResolver>(_ => new ManifestInheritanceResolver(manifestRoot));
        services.AddSingleton<IProviderManifestLoader>(_ =>
            new ProviderManifestLoader(
                _.GetRequiredService<IProviderManifestRegistry>(),
                _.GetRequiredService<IManifestSchemaValidator>(),
                _.GetRequiredService<IManifestInheritanceResolver>(),
                manifestRoot));
        services.AddSingleton<IManifestDefaultResolver, ManifestDefaultResolver>();
        services.AddSingleton<IManifestConditionEvaluator, ManifestConditionEvaluator>();
        services.AddSingleton<IManifestValidationEngine, ManifestValidationEngine>();
        services.AddSingleton<IManifestOutputMapper, ManifestOutputMapper>();
        return services;
    }
}
