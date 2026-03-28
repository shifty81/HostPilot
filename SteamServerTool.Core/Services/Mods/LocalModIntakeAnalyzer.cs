using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public class LocalModIntakeAnalyzer
{
    private readonly IReadOnlyList<IModMetadataReader> _readers;

    public LocalModIntakeAnalyzer(IEnumerable<IModMetadataReader>? readers = null)
    {
        _readers = readers?.ToList() ?? new List<IModMetadataReader>();
    }

    public async Task<List<ModImportCandidate>> AnalyzeAsync(ModImportRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.SourcePath))
            throw new ArgumentException("SourcePath is required.", nameof(request));

        var results = new List<ModImportCandidate>();

        if (File.Exists(request.SourcePath))
        {
            var candidate = await AnalyzeFileAsync(request, request.SourcePath, cancellationToken);
            results.Add(candidate);
            return results;
        }

        if (Directory.Exists(request.SourcePath))
        {
            var entries = Directory.GetFileSystemEntries(request.SourcePath);
            if (entries.Length == 0)
            {
                results.Add(new ModImportCandidate
                {
                    DisplayName = Path.GetFileName(request.SourcePath),
                    SourcePath = request.SourcePath,
                    SourceType = ModImportSourceType.LocalFolder,
                    PackageType = ModPackageType.FolderPackage,
                    RequiresReview = true,
                    Compatibility = ModCompatibilityLevel.Warning,
                    Warnings = { "Folder is empty." }
                });
                return results;
            }

            var acceptableFiles = Directory.EnumerateFiles(request.SourcePath, "*", SearchOption.AllDirectories)
                .Where(x => request.Rules.AcceptsExtension(Path.GetExtension(x)) || request.Rules.AllowRawInstall)
                .ToList();

            if (!request.Rules.AcceptFolders && acceptableFiles.Count == 0)
            {
                results.Add(new ModImportCandidate
                {
                    DisplayName = Path.GetFileName(request.SourcePath),
                    SourcePath = request.SourcePath,
                    SourceType = ModImportSourceType.LocalFolder,
                    PackageType = ModPackageType.FolderPackage,
                    RequiresReview = true,
                    Compatibility = ModCompatibilityLevel.Incompatible,
                    Warnings = { "This server type does not accept folder imports." }
                });
                return results;
            }

            if (acceptableFiles.Count > 1)
            {
                foreach (var file in acceptableFiles)
                    results.Add(await AnalyzeFileAsync(request, file, cancellationToken, request.SourcePath));
                return results;
            }

            results.Add(new ModImportCandidate
            {
                DisplayName = Path.GetFileName(request.SourcePath),
                SourcePath = request.SourcePath,
                RelativeSourcePath = Path.GetFileName(request.SourcePath),
                SourceType = ModImportSourceType.LocalFolder,
                PackageType = ModPackageType.FolderPackage,
                SuggestedTargetRelativePath = request.Rules.TargetPath,
                Compatibility = ModCompatibilityLevel.Compatible,
                RequiresReview = false,
            });
            return results;
        }

        throw new FileNotFoundException($"Source path not found: {request.SourcePath}");
    }

    private async Task<ModImportCandidate> AnalyzeFileAsync(ModImportRequest request, string filePath, CancellationToken cancellationToken, string? rootDirectory = null)
    {
        var extension = Path.GetExtension(filePath);
        var fileName = Path.GetFileName(filePath);
        var relativePath = rootDirectory is null ? fileName : Path.GetRelativePath(rootDirectory, filePath);

        var candidate = new ModImportCandidate
        {
            DisplayName = Path.GetFileNameWithoutExtension(filePath),
            SourcePath = filePath,
            RelativeSourcePath = relativePath,
            SourceType = request.SourceType == ModImportSourceType.Unknown ? ModImportSourceType.LocalFile : request.SourceType,
            DetectedExtension = extension,
            HashSha256 = File.Exists(filePath) ? HashUtility.ComputeSha256(filePath) : null,
        };

        if (string.Equals(extension, ".zip", StringComparison.OrdinalIgnoreCase))
        {
            candidate.PackageType = ModPackageType.Archive;
            candidate.RequiresExtraction = request.Rules.AutoExtractZip;
            candidate.SuggestedTargetRelativePath = request.Rules.TargetPath;
        }
        else if (request.Rules.AcceptsExtension(extension))
        {
            candidate.PackageType = ModPackageType.SingleModFile;
            candidate.SuggestedTargetRelativePath = request.Rules.TargetPath;
        }
        else
        {
            candidate.PackageType = ModPackageType.Unknown;
            candidate.RequiresReview = true;
            candidate.Compatibility = ModCompatibilityLevel.Warning;
            candidate.Warnings.Add($"File extension '{extension}' is not explicitly supported by this template.");
            candidate.SuggestedTargetRelativePath = request.Rules.AllowRawInstall
                ? request.Rules.TargetPath
                : null;
        }

        if (candidate.PackageType != ModPackageType.Unknown)
            candidate.Compatibility = ModCompatibilityLevel.Compatible;

        foreach (var reader in _readers)
        {
            if (!reader.CanRead(filePath)) continue;
            var enriched = await reader.TryReadAsync(filePath, cancellationToken);
            if (enriched is null) continue;

            candidate.DisplayName = string.IsNullOrWhiteSpace(enriched.DisplayName) ? candidate.DisplayName : enriched.DisplayName;
            candidate.DetectedLoader = enriched.DetectedLoader ?? candidate.DetectedLoader;
            candidate.DetectedVersion = enriched.DetectedVersion ?? candidate.DetectedVersion;
            candidate.DetectedDependencies.AddRange(enriched.DetectedDependencies);
            candidate.Warnings.AddRange(enriched.Warnings);
            break;
        }

        return candidate;
    }
}
