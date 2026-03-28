using HostPilot.Core.Models.Discovery;

namespace HostPilot.Core.Services.Discovery;

public sealed class InstalledServerDiscoveryService
{
    private readonly IReadOnlyList<IServerFootprintDetector> _detectors;
    private readonly DiscoveryRootLocator _rootLocator;

    public InstalledServerDiscoveryService(IEnumerable<IServerFootprintDetector> detectors, DiscoveryRootLocator? rootLocator = null)
    {
        _detectors = detectors.ToList();
        _rootLocator = rootLocator ?? new DiscoveryRootLocator();
    }

    public async Task<IReadOnlyList<DiscoveredServerCandidate>> DiscoverAsync(
        IEnumerable<string>? additionalRoots = null,
        string? serversBasePath = null,
        CancellationToken cancellationToken = default)
    {
        var roots = _rootLocator.GetSuggestedRoots(serversBasePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var root in additionalRoots ?? Array.Empty<string>())
        {
            if (Directory.Exists(root))
            {
                roots.Add(root);
            }
        }

        var discovered = new List<DiscoveredServerCandidate>();

        foreach (var detector in _detectors)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var matches = await detector.ScanAsync(roots, cancellationToken).ConfigureAwait(false);
            discovered.AddRange(matches);
        }

        return discovered
            .GroupBy(x => x.InstallPath, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(x => x.Confidence).First())
            .OrderByDescending(x => x.Confidence)
            .ThenBy(x => x.DisplayName)
            .ToList();
    }

    public static IReadOnlyList<IServerFootprintDetector> CreateDefaultDetectors()
    {
        return new IServerFootprintDetector[]
        {
            new SignatureBasedDetector(new ServerDiscoverySignature
            {
                ServerType = "steamcmd-generic",
                DisplayName = "Steam Dedicated Server",
                ExecutableNames = new() { "steamcmd.exe" },
                RelativeFolders = new() { "steamapps", "steamapps\\common" }
            }),
            new SignatureBasedDetector(new ServerDiscoverySignature
            {
                ServerType = "minecraft-java",
                DisplayName = "Minecraft Java Server",
                ExecutableNames = new() { "server.jar", "fabric-server-launch.jar", "forge-1.20.1.jar" },
                RequiredFiles = new() { "eula.txt", "server.properties" },
                ConfigCandidates = new() { "server.properties" },
                SaveCandidates = new() { "world", "world_nether", "world_the_end" }
            }),
            new SignatureBasedDetector(new ServerDiscoverySignature
            {
                ServerType = "vintagestory",
                DisplayName = "Vintage Story Server",
                ExecutableNames = new() { "VintagestoryServer.exe" },
                RequiredFiles = new() { "serverconfig.json" },
                ConfigCandidates = new() { "serverconfig.json" },
                SaveCandidates = new() { "Data", "Mods", "Saves" }
            })
        };
    }
}
