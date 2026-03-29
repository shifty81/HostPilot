using System.Text.RegularExpressions;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Parsers;

public sealed class PalworldLogParser : RegexParserBase
{
    public override string ProviderId => "palworld";

    protected override IReadOnlyList<(Regex regex, Func<Match, ParsedLogEvent> map)> Rules =>
    [
        (Rx(@"Initialized\s+IIServer|Game\s+Server\s+Initialized|Listening on port"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Palworld server reported ready.", IndicatesReady = true, Severity = LogSeverity.Success }),
        (Rx(@"Saving world|shutdown"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Palworld server is saving or shutting down.", IndicatesStopping = true }),
        (Rx(@"error|fatal|exception|crash"), m => new ParsedLogEvent { Category = "Crash", Message = m.Value, RawLine = m.Value, IndicatesCrash = true, Severity = LogSeverity.Error })
    ];
}
