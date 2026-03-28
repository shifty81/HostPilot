using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public class LocalModInstallPlanner
{
    public ModInstallPlan BuildPlan(ModImportRequest request, IEnumerable<ModImportCandidate> candidates)
    {
        var plan = new ModInstallPlan
        {
            ServerName = request.ServerName,
            ServerDirectory = request.ServerDirectory,
            RequiresRestart = request.Rules.RequiresRestart,
        };

        foreach (var candidate in candidates)
        {
            var item = new ModInstallPlanItem
            {
                SourcePath = candidate.SourcePath,
                DisplayName = candidate.DisplayName,
                OverwriteExisting = request.OverwriteExisting,
            };

            if (candidate.RequiresReview && !request.Rules.AllowRawInstall)
            {
                item.Action = ModInstallAction.Reject;
                item.Warnings.AddRange(candidate.Warnings);
                item.Warnings.Add("Manual review required before install.");
                plan.Items.Add(item);
                continue;
            }

            var targetRoot = Path.Combine(request.ServerDirectory, candidate.SuggestedTargetRelativePath ?? request.Rules.TargetPath);
            var destinationFileName = Path.GetFileName(candidate.SourcePath);
            item.DestinationPath = candidate.PackageType == ModPackageType.FolderPackage
                ? targetRoot
                : Path.Combine(targetRoot, destinationFileName);

            item.Action = candidate.PackageType switch
            {
                ModPackageType.Archive when candidate.RequiresExtraction => ModInstallAction.ExtractArchive,
                ModPackageType.Archive => ModInstallAction.CopyAsIs,
                ModPackageType.FolderPackage or ModPackageType.MultiModFolder => ModInstallAction.CopyDirectory,
                ModPackageType.SingleModFile => ModInstallAction.CopyAsIs,
                ModPackageType.MixedPayload => ModInstallAction.NeedsReview,
                _ => request.Rules.AllowRawInstall ? ModInstallAction.CopyAsIs : ModInstallAction.Reject,
            };

            item.Warnings.AddRange(candidate.Warnings);
            plan.Items.Add(item);
        }

        return plan;
    }
}
