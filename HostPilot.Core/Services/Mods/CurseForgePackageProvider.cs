using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public sealed class CurseForgePackageProvider : IModPackageProvider
{
    public string ProviderId => "curseforge";

    public Task<IReadOnlyList<ModPackage>> SearchAsync(ModSearchQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ModPackage>>(
        [
            new ModPackage
            {
                ProviderId = ProviderId,
                PackageId = "sample-curseforge-project",
                DisplayName = "CurseForge sample project",
                Version = "1.0.0",
                Summary = "Wire real API client in next pass",
                GameId = query.GameId
            }
        ]);
    }

    public Task<ModPackage?> GetAsync(string packageId, string gameId, CancellationToken cancellationToken = default)
        => Task.FromResult<ModPackage?>(new ModPackage { ProviderId = ProviderId, PackageId = packageId, DisplayName = packageId, GameId = gameId });

    public Task<ModDependencyResult> ResolveDependenciesAsync(ModPackage package, CancellationToken cancellationToken = default)
        => Task.FromResult(new ModDependencyResult { ResolvedPackages = [package] });

    public Task<string> DownloadAsync(ModPackage package, string targetDirectory, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(targetDirectory);
        var path = Path.Combine(targetDirectory, $"{package.PackageId}.cf.txt");
        File.WriteAllText(path, "Placeholder CurseForge download.");
        return Task.FromResult(path);
    }
}
