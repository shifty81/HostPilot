using System.Text.RegularExpressions;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Parsers;

public abstract class RegexParserBase : IProviderLogParser
{
    public abstract string ProviderId { get; }
    protected abstract IReadOnlyList<(Regex regex, Func<Match, ParsedLogEvent> map)> Rules { get; }

    public virtual ParsedLogEvent Parse(string line)
    {
        foreach (var (regex, map) in Rules)
        {
            var match = regex.Match(line);
            if (match.Success)
                return map(match);
        }

        return new ParsedLogEvent
        {
            Message = line.Trim(),
            RawLine = line
        };
    }

    protected static Regex Rx(string pattern)
        => new(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
}
