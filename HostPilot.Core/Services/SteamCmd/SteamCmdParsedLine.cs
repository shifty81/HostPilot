namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdParsedLine
{
    public SteamCmdLineType LineType { get; init; } = SteamCmdLineType.Unknown;
    public string RawLine { get; init; } = string.Empty;
    public int? Percent { get; init; }

    public bool IsTerminalSuccess => LineType == SteamCmdLineType.Success;
    public bool IsTerminalFailure => LineType == SteamCmdLineType.Error;
}
