using HostPilot.Core.Models.Discovery;

namespace HostPilot.Core.Services.Discovery;

public interface IServerFootprintDetector
{
    string ServerType { get; }
    Task<IReadOnlyList<DiscoveredServerCandidate>> ScanAsync(IEnumerable<string> roots, CancellationToken cancellationToken = default);
}
