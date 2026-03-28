using HostPilot.Core.Models.Discovery;

namespace HostPilot.Core.Services.Discovery;

public class ManifestBackedServerDetector
{
    public IReadOnlyList<DiscoveredServerCandidate> ScanPaths(
        IEnumerable<string> roots,
        IEnumerable<ServerDetectionSignature> signatures)
    {
        var candidates = new List<DiscoveredServerCandidate>();

        foreach (var root in roots.Where(Directory.Exists).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            foreach (var directory in EnumerateDirectoriesSafe(root))
            {
                foreach (var signature in signatures)
                {
                    var evidence = CollectEvidence(directory, signature);
                    if (evidence.Count < signature.MinimumEvidence)
                        continue;

                    var executablePath = signature.Executables
                        .Select(name => Directory.EnumerateFiles(directory, name, SearchOption.AllDirectories).FirstOrDefault())
                        .FirstOrDefault(path => !string.IsNullOrWhiteSpace(path)) ?? string.Empty;

                    candidates.Add(new DiscoveredServerCandidate
                    {
                        ManifestId = signature.ManifestId,
                        ServerType = signature.ManifestId,
                        DisplayName = signature.DisplayName,
                        InstallPath = directory,
                        ExecutablePath = executablePath,
                        Confidence = Math.Min(0.99, 0.45 + evidence.Count * 0.15),
                        Evidence = evidence,
                        SuggestedConfigFiles = signature.ConfigFiles.ToList(),
                    });
                }
            }
        }

        return candidates
            .GroupBy(c => $"{c.ManifestId}|{c.InstallPath}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(x => x.Confidence).First())
            .OrderByDescending(c => c.Confidence)
            .ToList();
    }

    private static List<string> CollectEvidence(string directory, ServerDetectionSignature signature)
    {
        var evidence = new List<string>();

        foreach (var folder in signature.Folders)
        {
            if (directory.Contains(folder, StringComparison.OrdinalIgnoreCase))
                evidence.Add($"Matched folder marker: {folder}");
        }

        foreach (var file in signature.Files)
        {
            if (File.Exists(Path.Combine(directory, file)) ||
                Directory.EnumerateFiles(directory, file, SearchOption.AllDirectories).Any())
                evidence.Add($"Found file: {file}");
        }

        foreach (var executable in signature.Executables)
        {
            if (Directory.EnumerateFiles(directory, executable, SearchOption.AllDirectories).Any())
                evidence.Add($"Found executable: {executable}");
        }

        return evidence.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static IEnumerable<string> EnumerateDirectoriesSafe(string root)
    {
        yield return root;

        var pending = new Queue<string>();
        pending.Enqueue(root);

        while (pending.Count > 0)
        {
            var current = pending.Dequeue();
            IEnumerable<string> children;
            try
            {
                children = Directory.EnumerateDirectories(current);
            }
            catch
            {
                continue;
            }

            foreach (var child in children)
            {
                yield return child;
                pending.Enqueue(child);
            }
        }
    }
}
