using HostPilot.Core.Services.Mods;
using Xunit;

namespace HostPilot.Tests;

public sealed class DependencyReviewServiceTests
{
    [Fact]
    public void Marks_Installed_Dependencies_As_AlreadyInstalled()
    {
        var service = new DependencyReviewService();
        var installed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "architectury" };

        var items = service.BuildReviewItems(
            new[]
            {
                ("architectury", "Architectury API", "1.0.0", true, "Required by selected mod"),
                ("cloth-config", "Cloth Config", "2.0.0", false, "Recommended")
            },
            installed,
            "curseforge");

        Assert.True(items.Single(x => x.DependencyId == "architectury").IsAlreadyInstalled);
        Assert.False(items.Single(x => x.DependencyId == "cloth-config").IsAlreadyInstalled);
    }
}
