using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Abstractions;

public interface IProviderLogParser
{
    string ProviderId { get; }
    ParsedLogEvent Parse(string line);
}
