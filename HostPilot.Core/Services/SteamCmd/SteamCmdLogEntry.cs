namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdLogEntry
{
    public string Message { get; init; } = string.Empty;
    public bool IsError { get; init; }
}
