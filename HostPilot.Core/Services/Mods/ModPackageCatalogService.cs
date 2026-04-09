using System.IO.Compression;
using System.Security.Cryptography;
using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public sealed class ModPackageCatalogService : IModCatalogService
{
    private readonly IReadOnlyDictionary<string, IModPackageProvider> _providers;

    public ModPackageCatalogService(IEnumerable<IModPackageProvider> providers)
    {
        _providers = providers.ToDictionary(x => x.ProviderId, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<ModPackage>> SearchAsync(ModSearchQuery query, CancellationToken cancellationToken = default)
    {
        if (!_providers.TryGetValue(query.ProviderId, out var provider))
        {
            return Array.Empty<ModPackage>();
        }

        return await provider.SearchAsync(query, cancellationToken);
    }

    public async Task<ModDependencyResult> ResolveDependenciesAsync(ModPackage package, CancellationToken cancellationToken = default)
    {
        if (!_providers.TryGetValue(package.ProviderId, out var provider))
        {
            return new ModDependencyResult { Warnings = { $"Unknown provider: {package.ProviderId}" } };
        }

        return await provider.ResolveDependenciesAsync(package, cancellationToken);
    }

    public Task<IReadOnlyList<ModPackage>> ImportLocalAsync(LocalModImportRequest request, CancellationToken cancellationToken = default)
    {
        var results = new List<ModPackage>();

        foreach (var rawPath in request.SourcePaths)
        {
            // Canonicalize and validate path to prevent directory traversal
            string sourcePath;
            try
            {
                sourcePath = Path.GetFullPath(rawPath);
            }
            catch (Exception)
            {
                continue;
            }

            if (!File.Exists(sourcePath))
                continue;
            var package = new ModPackage
            {
                ProviderId = "local",
                PackageId = Path.GetFileNameWithoutExtension(sourcePath),
                DisplayName = Path.GetFileNameWithoutExtension(sourcePath),
                Version = "imported",
                GameId = request.GameId,
                Summary = "Imported from local file",
                Hash = ComputeHash(sourcePath),
                Metadata = { ["sourcePath"] = sourcePath }
            };

            if (Path.GetExtension(sourcePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                package.Metadata["container"] = "zip";
                using var archive = ZipFile.OpenRead(sourcePath);
                package.Metadata["entryCount"] = archive.Entries.Count.ToString();
            }

            results.Add(package);
        }

        foreach (var rawDir in request.SourcePaths)
        {
            string sourcePath;
            try { sourcePath = Path.GetFullPath(rawDir); }
            catch (Exception) { continue; }

            if (!Directory.Exists(sourcePath))
                continue;

            results.Add(new ModPackage
            {
                ProviderId = "local",
                PackageId = new DirectoryInfo(sourcePath).Name,
                DisplayName = new DirectoryInfo(sourcePath).Name,
                Version = "folder-import",
                GameId = request.GameId,
                Summary = "Imported from local folder",
                Metadata = { ["sourcePath"] = sourcePath, ["container"] = "folder" }
            });
        }

        return Task.FromResult<IReadOnlyList<ModPackage>>(results);
    }

    private static string ComputeHash(string path)
    {
        using var stream = File.OpenRead(path);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(stream));
    }
}
