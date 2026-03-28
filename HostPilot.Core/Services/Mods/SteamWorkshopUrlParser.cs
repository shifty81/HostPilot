using System.Text.RegularExpressions;

namespace HostPilot.Core.Services.Mods;

public static partial class SteamWorkshopUrlParser
{
    private static readonly Regex NumericIdRegex = WorkshopIdRegex();
    private static readonly Regex UrlIdRegex = WorkshopUrlRegex();

    public static string? ExtractPublishedFileId(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        raw = raw.Trim();
        if (NumericIdRegex.IsMatch(raw))
            return raw;

        var match = UrlIdRegex.Match(raw);
        if (!match.Success)
            return null;

        return match.Groups[1].Value;
    }

    [GeneratedRegex("^[0-9]{5,20}$", RegexOptions.Compiled)]
    private static partial Regex WorkshopIdRegex();

    [GeneratedRegex(@"[?&]id=([0-9]{5,20})", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex WorkshopUrlRegex();
}
