using System.IO;
using System.Text.Json;

namespace HostPilot.Core.Services.DynamicForms;

public sealed class ManifestLoader
{
    public ManifestDocument LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public ManifestDocument LoadFromJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var manifest = JsonSerializer.Deserialize<ManifestDocument>(json, options);
        return manifest ?? new ManifestDocument();
    }
}
