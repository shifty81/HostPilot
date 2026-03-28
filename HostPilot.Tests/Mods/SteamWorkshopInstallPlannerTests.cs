using HostPilot.Core.Models.Mods;
using HostPilot.Core.Services.Mods;

namespace HostPilot.Tests.Mods;

public sealed class SteamWorkshopInstallPlannerTests
{
    [Fact]
    public void BuildSteamCmdInvocation_FormatsWorkshopDownloadCommand()
    {
        var request = new SteamWorkshopInstallRequest
        {
            AppId = 346110,
            PublishedFileId = "3074561085",
            SteamCmdPath = "steamcmd.exe",
            Login = "anonymous",
            Validate = true,
        };

        var (fileName, arguments) = SteamWorkshopInstallPlanner.BuildSteamCmdInvocation(request);

        Assert.Equal("steamcmd.exe", fileName);
        Assert.Contains("+workshop_download_item 346110 3074561085 validate", arguments);
        Assert.EndsWith("+quit", arguments);
    }
}
