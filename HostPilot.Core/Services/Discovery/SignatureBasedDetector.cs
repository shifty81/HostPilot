using HostPilot.Core.Models.Discovery;

namespace HostPilot.Core.Services.Discovery;

public sealed class SignatureBasedDetector : IServerFootprintDetector
{
    private readonly ServerDiscoverySignature _signature;

    public SignatureBasedDetector(ServerDiscoverySignature signature)
    {
        _signature = signature;
    }

    public string ServerType => _signature.ServerType;

    public Task<IReadOnlyList<DiscoveredServerCandidate>> ScanAsync(IEnumerable<string> roots, CancellationToken cancellationToken = default)
    {
        var results = new List<DiscoveredServerCandidate>();

        foreach (var root in roots.Where(Directory.Exists))
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var folder in EnumerateCandidateDirectories(root))
            {
                var executable = _signature.ExecutableNames
                    .Select(name => Path.Combine(folder, name))
                    .FirstOrDefault(File.Exists);

                if (executable is null)
                {
                    continue;
                }

                var evidence = new List<string> { $"Found executable: {Path.GetFileName(executable)}" };
                var confidence = 0.50;

                foreach (var file in _signature.RequiredFiles)
                {
                    var path = Path.Combine(folder, file);
                    if (File.Exists(path))
                    {
                        evidence.Add($"Found required file: {file}");
                        confidence += 0.15;
                    }
                }

                foreach (var file in _signature.OptionalFiles)
                {
                    var path = Path.Combine(folder, file);
                    if (File.Exists(path))
                    {
                        evidence.Add($"Found optional file: {file}");
                        confidence += 0.05;
                    }
                }

                var configPath = _signature.ConfigCandidates
                    .Select(path => Path.Combine(folder, path))
                    .FirstOrDefault(File.Exists);

                var savePath = _signature.SaveCandidates
                    .Select(path => Path.Combine(folder, path))
                    .FirstOrDefault(Directory.Exists);

                results.Add(new DiscoveredServerCandidate
                {
                    ServerType = _signature.ServerType,
                    DisplayName = _signature.DisplayName,
                    InstallPath = folder,
                    ExecutablePath = executable,
                    ConfigPath = configPath,
                    SavePath = savePath,
                    Confidence = Math.Min(confidence, 0.99),
                    Evidence = evidence
                });
            }
        }

        return Task.FromResult<IReadOnlyList<DiscoveredServerCandidate>>(results
            .GroupBy(x => x.InstallPath, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(x => x.Confidence).First())
            .OrderByDescending(x => x.Confidence)
            .ToList());
    }

    private IEnumerable<string> EnumerateCandidateDirectories(string root)
    {
        yield return root;

        foreach (var relative in _signature.RelativeFolders)
        {
            var combined = Path.Combine(root, relative);
            if (Directory.Exists(combined))
            {
                yield return combined;
            }
        }

        IEnumerable<string> children;
        try
        {
            children = Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly);
        }
        catch
        {
            yield break;
        }

        foreach (var child in children)
        {
            yield return child;
        }
    }
}
