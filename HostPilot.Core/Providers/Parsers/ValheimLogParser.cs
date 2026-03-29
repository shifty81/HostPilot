using System.Text.RegularExpressions;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Parsers;

public sealed class ValheimLogParser : RegexParserBase
{
    public override string ProviderId => "valheim";

    protected override IReadOnlyList<(Regex regex, Func<Match, ParsedLogEvent> map)> Rules =>
    [
        (Rx(@"Game server connected|Session "), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Valheim server reported ready.", IndicatesReady = true, Severity = LogSeverity.Success }),
        (Rx(@"World saved|Shutdown"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Valheim server is saving or shutting down.", IndicatesStopping = true }),
        (Rx(@"error|exception|crash"), m => new ParsedLogEvent { Category = "Crash", Message = m.Value, RawLine = m.Value, IndicatesCrash = true, Severity = LogSeverity.Error })
    ];
}
