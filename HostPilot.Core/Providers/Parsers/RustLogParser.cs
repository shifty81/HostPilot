using System.Text.RegularExpressions;
using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Parsers;

public sealed class RustLogParser : RegexParserBase
{
    public override string ProviderId => "rust";

    protected override IReadOnlyList<(Regex regex, Func<Match, ParsedLogEvent> map)> Rules =>
    [
        (Rx(@"Server\s+startup\s+complete|RCon\s+started"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Rust server reported ready.", IndicatesReady = true, Severity = LogSeverity.Success }),
        (Rx(@"saving|shutdown"), _ => new ParsedLogEvent { Category = "Lifecycle", Message = "Rust server is saving or shutting down.", IndicatesStopping = true }),
        (Rx(@"exception|crash|fatal"), m => new ParsedLogEvent { Category = "Crash", Message = m.Value, RawLine = m.Value, IndicatesCrash = true, Severity = LogSeverity.Error })
    ];
}
