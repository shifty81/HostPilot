using System.Text.Json;
using HostPilot.Core.Models;

namespace HostPilot.Core.Services.Deployment;

public class DeploymentManifestRegistry
{
    private readonly string _manifestDirectory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public DeploymentManifestRegistry(string manifestDirectory)
    {
        _manifestDirectory = manifestDirectory;
    }

    public IReadOnlyList<DeploymentManifest> LoadAll()
    {
        if (!Directory.Exists(_manifestDirectory))
            return Array.Empty<DeploymentManifest>();

        var manifests = new List<DeploymentManifest>();
        foreach (var file in Directory.EnumerateFiles(_manifestDirectory, "*.json", SearchOption.TopDirectoryOnly)
                                      .OrderBy(Path.GetFileName))
        {
            try
            {
                var json = File.ReadAllText(file);
                var manifest = JsonSerializer.Deserialize<DeploymentManifest>(json, _jsonOptions);
                if (manifest is not null)
                    manifests.Add(manifest);
            }
            catch (Exception)
            {
                // Skip malformed or unreadable manifest files; continue loading the rest.
            }
        }

        return manifests;
    }

    public DeploymentManifest? LoadById(string manifestId)
    {
        var path = Path.Combine(_manifestDirectory, $"{manifestId}.json");
        if (!File.Exists(path))
            return null;

        return JsonSerializer.Deserialize<DeploymentManifest>(File.ReadAllText(path), _jsonOptions);
    }
}
