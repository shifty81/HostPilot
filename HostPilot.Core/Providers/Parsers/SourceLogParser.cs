using System.Text.RegularExpressions;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Parsers;

public sealed class SourceLogParser : RegexParserBase
{
    public override string ProviderId => "source-engine";

    protected override IReadOnlyList<(Regex regex, Func<Match, ParsedLogEvent> map)> Rules =>
    [
        (Rx(@"Connection to Steam servers successful|VAC secure mode is activated"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Source server reported ready.", IndicatesReady = true, Severity = LogSeverity.Success }),
        (Rx(@"quit|shutdown|server is hibernating"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Source server is stopping or idle.", IndicatesStopping = true }),
        (Rx(@"error|assert|fatal"), m => new ParsedLogEvent { Category = "Crash", Message = m.Value, RawLine = m.Value, IndicatesCrash = true, Severity = LogSeverity.Error })
    ];
}
