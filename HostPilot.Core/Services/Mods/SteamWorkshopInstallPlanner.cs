using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public static class SteamWorkshopInstallPlanner
{
    public static (string FileName, string Arguments) BuildSteamCmdInvocation(SteamWorkshopInstallRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.PublishedFileId);
        if (request.AppId == 0)
            throw new ArgumentOutOfRangeException(nameof(request.AppId), "AppId must be greater than zero.");

        var validateArg = request.Validate ? " validate" : string.Empty;
        var arguments = $"+login {request.Login} +workshop_download_item {request.AppId} {request.PublishedFileId}{validateArg} +quit";
        return (request.SteamCmdPath, arguments);
    }
}
