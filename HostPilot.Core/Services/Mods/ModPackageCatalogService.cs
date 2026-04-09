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
            var sourcePath = SafeResolvePath(rawPath);
            if (sourcePath is null || !File.Exists(sourcePath))
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
            var sourcePath = SafeResolvePath(rawDir);
            if (sourcePath is null || !Directory.Exists(sourcePath))
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

    /// <summary>
    /// Resolves <paramref name="rawPath"/> to an absolute, canonical path.
    /// Returns <see langword="null"/> if the value is null, empty, or contains
    /// invalid characters (including null bytes that could bypass OS-level checks).
    /// </summary>
    private static string? SafeResolvePath(string? rawPath)
    {
        if (string.IsNullOrWhiteSpace(rawPath))
            return null;

        // Reject embedded null bytes which some OS implementations ignore after this point
        if (rawPath.Contains('\0'))
            return null;

        try
        {
            var full = Path.GetFullPath(rawPath);
            // Path must be absolute after resolution
            return Path.IsPathRooted(full) ? full : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string ComputeHash(string path)
    {
        using var stream = File.OpenRead(path);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(stream));
    }
}
