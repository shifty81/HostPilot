namespace HostPilot.Providers.Manifesting.Enums;

public enum ManifestOperationType
{
    None = 0,
    SteamCmdInstall = 1,
    SteamCmdUpdate = 2,
    ExecutableStart = 3,
    RconStop = 4,
    ProcessStop = 5,
    Custom = 6
}
