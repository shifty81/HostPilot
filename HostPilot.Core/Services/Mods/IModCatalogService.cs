using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public interface IModCatalogService
{
    Task<IReadOnlyList<ModPackage>> SearchAsync(ModSearchQuery query, CancellationToken cancellationToken = default);
    Task<ModDependencyResult> ResolveDependenciesAsync(ModPackage package, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModPackage>> ImportLocalAsync(LocalModImportRequest request, CancellationToken cancellationToken = default);
}
