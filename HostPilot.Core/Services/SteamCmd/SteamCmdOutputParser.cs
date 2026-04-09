using System.Text.RegularExpressions;

namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdOutputParser
{
    private static readonly Regex ProgressRegex =
        new(@"\((\d+)%\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SteamCmdParsedLine Parse(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return new SteamCmdParsedLine { LineType = SteamCmdLineType.Unknown, RawLine = line ?? string.Empty };

        var trimmed = line.Trim();

        if (trimmed.StartsWith("Success", StringComparison.OrdinalIgnoreCase))
            return new SteamCmdParsedLine { LineType = SteamCmdLineType.Success, RawLine = line };

        if (trimmed.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
            return new SteamCmdParsedLine { LineType = SteamCmdLineType.Error, RawLine = line };

        var match = ProgressRegex.Match(trimmed);
        if (match.Success && int.TryParse(match.Groups[1].Value, out var pct))
            return new SteamCmdParsedLine { LineType = SteamCmdLineType.Progress, RawLine = line, Percent = pct };

        return new SteamCmdParsedLine { LineType = SteamCmdLineType.Unknown, RawLine = line };
    }
}
