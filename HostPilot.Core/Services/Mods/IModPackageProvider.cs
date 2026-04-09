using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public interface IModPackageProvider
{
    string ProviderId { get; }
    Task<IReadOnlyList<ModPackage>> SearchAsync(ModSearchQuery query, CancellationToken cancellationToken = default);
    Task<ModPackage?> GetAsync(string packageId, string gameId, CancellationToken cancellationToken = default);
    Task<ModDependencyResult> ResolveDependenciesAsync(ModPackage package, CancellationToken cancellationToken = default);
    Task<string> DownloadAsync(ModPackage package, string targetDirectory, CancellationToken cancellationToken = default);
}
