using System.IO.Compression;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public class LocalModImportService
{
    private readonly LocalModIntakeAnalyzer _analyzer;
    private readonly LocalModInstallPlanner _planner;
    private readonly InstalledModRegistryService _registry;

    public LocalModImportService(
        LocalModIntakeAnalyzer? analyzer = null,
        LocalModInstallPlanner? planner = null,
        InstalledModRegistryService? registry = null)
    {
        _analyzer = analyzer ?? new LocalModIntakeAnalyzer();
        _planner = planner ?? new LocalModInstallPlanner();
        _registry = registry ?? new InstalledModRegistryService();
    }

    public async Task<ModInstallPlan> PreviewAsync(ModImportRequest request, CancellationToken cancellationToken = default)
    {
        var candidates = await _analyzer.AnalyzeAsync(request, cancellationToken);
        return _planner.BuildPlan(request, candidates);
    }

    public async Task<ModInstallPlan> ExecuteAsync(ModImportRequest request, string registryPath, CancellationToken cancellationToken = default)
    {
        var candidates = await _analyzer.AnalyzeAsync(request, cancellationToken);
        var plan = _planner.BuildPlan(request, candidates);
        if (plan.HasBlockingIssues) return plan;

        foreach (var item in plan.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var destinationDir = item.Action == ModInstallAction.CopyDirectory
                ? item.DestinationPath
                : Path.GetDirectoryName(item.DestinationPath)!;
            Directory.CreateDirectory(destinationDir);

            switch (item.Action)
            {
                case ModInstallAction.CopyAsIs:
                    File.Copy(item.SourcePath, item.DestinationPath, item.OverwriteExisting);
                    break;
                case ModInstallAction.CopyDirectory:
                    CopyDirectory(item.SourcePath, item.DestinationPath, item.OverwriteExisting);
                    break;
                case ModInstallAction.ExtractArchive:
                    if (Directory.Exists(item.DestinationPath) && item.OverwriteExisting)
                        Directory.Delete(item.DestinationPath, true);
                    Directory.CreateDirectory(item.DestinationPath);
                    ZipFile.ExtractToDirectory(item.SourcePath, item.DestinationPath, item.OverwriteExisting);
                    break;
                case ModInstallAction.NeedsReview:
                case ModInstallAction.Reject:
                    continue;
            }
        }

        var records = _registry.Load(registryPath);
        foreach (var item in plan.Items.Where(x => x.Action is ModInstallAction.CopyAsIs or ModInstallAction.CopyDirectory or ModInstallAction.ExtractArchive))
        {
            var sourceHash = File.Exists(item.SourcePath) ? HashUtility.ComputeSha256(item.SourcePath) : null;
            _registry.Upsert(records, new InstalledModRecord
            {
                ServerName = request.ServerName,
                Name = item.DisplayName,
                SourcePath = item.SourcePath,
                InstalledPath = item.DestinationPath,
                HashSha256 = sourceHash,
                RequiresRestart = request.Rules.RequiresRestart,
                Provider = request.ProviderName,
                Source = "local",
                Managed = true,
            });
        }

        _registry.Save(registryPath, records);
        return plan;
    }

    private static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (var directory in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(directory.Replace(sourceDir, destinationDir));

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var destinationFile = file.Replace(sourceDir, destinationDir);
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);
            File.Copy(file, destinationFile, overwrite);
        }
    }
}
