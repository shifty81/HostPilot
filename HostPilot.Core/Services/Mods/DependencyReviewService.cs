using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public sealed class DependencyReviewService
{
    public IReadOnlyList<DependencyReviewItem> BuildReviewItems(
        IEnumerable<(string Id, string Name, string Version, bool Required, string Reason)> dependencies,
        ISet<string>? installedIds = null,
        string provider = "")
    {
        installedIds ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return dependencies
            .Select(x => new DependencyReviewItem
            {
                DependencyId = x.Id,
                DisplayName = x.Name,
                Version = x.Version,
                Provider = provider,
                IsRequired = x.Required,
                IsAlreadyInstalled = installedIds.Contains(x.Id),
                IsSelected = !installedIds.Contains(x.Id),
                Reason = x.Reason
            })
            .OrderByDescending(x => x.IsRequired)
            .ThenBy(x => x.DisplayName)
            .ToList();
    }
}
