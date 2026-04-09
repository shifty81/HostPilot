using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public sealed class ModrinthProvider : IModPackageProvider
{
    public string ProviderId => "modrinth";

    public Task<IReadOnlyList<ModPackage>> SearchAsync(ModSearchQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ModPackage>>(
        [
            new ModPackage
            {
                ProviderId = ProviderId,
                PackageId = "sample-modrinth-project",
                DisplayName = "Modrinth sample project",
                Version = "1.0.0",
                Summary = "Starter provider implementation",
                GameId = query.GameId
            }
        ]);

    public Task<ModPackage?> GetAsync(string packageId, string gameId, CancellationToken cancellationToken = default)
        => Task.FromResult<ModPackage?>(new ModPackage { ProviderId = ProviderId, PackageId = packageId, DisplayName = packageId, GameId = gameId });

    public Task<ModDependencyResult> ResolveDependenciesAsync(ModPackage package, CancellationToken cancellationToken = default)
        => Task.FromResult(new ModDependencyResult { ResolvedPackages = [package] });

    public Task<string> DownloadAsync(ModPackage package, string targetDirectory, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(targetDirectory);
        var path = Path.Combine(targetDirectory, $"{package.PackageId}.mr.txt");
        File.WriteAllText(path, "Placeholder Modrinth download.");
        return Task.FromResult(path);
    }
}
