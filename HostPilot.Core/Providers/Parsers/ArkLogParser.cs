using System.Text.RegularExpressions;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Parsers;

public sealed class ArkLogParser : RegexParserBase
{
    public override string ProviderId => "ark-survival-ascended";

    protected override IReadOnlyList<(Regex regex, Func<Match, ParsedLogEvent> map)> Rules =>
    [
        (Rx(@"Server\s+started|ShooterGameMode.*begun play"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "ARK server reported ready.", IndicatesReady = true, Severity = LogSeverity.Success }),
        (Rx(@"shutdown|saving world"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "ARK is shutting down or saving.", IndicatesStopping = true }),
        (Rx(@"assert|fatal|crash"), m => new ParsedLogEvent { Category = "Crash", Message = m.Value, RawLine = m.Value, IndicatesCrash = true, Severity = LogSeverity.Error })
    ];
}
