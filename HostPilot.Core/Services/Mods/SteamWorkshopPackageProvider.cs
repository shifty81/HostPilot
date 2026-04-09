using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public sealed class SteamWorkshopPackageProvider : IModPackageProvider
{
    public string ProviderId => "steam-workshop";

    public Task<IReadOnlyList<ModPackage>> SearchAsync(ModSearchQuery query, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ModPackage> results =
        [
            new ModPackage
            {
                ProviderId = ProviderId,
                PackageId = "sample-workshop-item",
                DisplayName = $"Workshop sample for {query.GameId}",
                Version = "1.0.0",
                Summary = $"Starter search result for '{query.QueryText}'",
                GameId = query.GameId,
                WebsiteUrl = "https://steamcommunity.com/workshop/"
            }
        ];

        return Task.FromResult(results);
    }

    public Task<ModPackage?> GetAsync(string packageId, string gameId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ModPackage?>(new ModPackage
        {
            ProviderId = ProviderId,
            PackageId = packageId,
            DisplayName = packageId,
            Version = "1.0.0",
            GameId = gameId
        });
    }

    public Task<ModDependencyResult> ResolveDependenciesAsync(ModPackage package, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ModDependencyResult
        {
            ResolvedPackages = [package]
        });
    }

    public Task<string> DownloadAsync(ModPackage package, string targetDirectory, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(targetDirectory);
        var outPath = Path.Combine(targetDirectory, $"{package.PackageId}.txt");
        File.WriteAllText(outPath, $"Placeholder download for {package.DisplayName}");
        return Task.FromResult(outPath);
    }
}
