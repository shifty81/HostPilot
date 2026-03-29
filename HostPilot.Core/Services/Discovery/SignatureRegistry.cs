using System.Text.Json;

namespace HostPilot.Core.Services.Discovery;

public class SignatureRegistry
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public SignatureRegistry(string directory)
    {
        _directory = directory;
    }

    public IReadOnlyList<ServerDetectionSignature> LoadAll()
    {
        if (!Directory.Exists(_directory))
            return Array.Empty<ServerDetectionSignature>();

        var signatures = new List<ServerDetectionSignature>();
        foreach (var path in Directory.EnumerateFiles(_directory, "*.json", SearchOption.TopDirectoryOnly)
                                      .OrderBy(Path.GetFileName))
        {
            try
            {
                var sig = JsonSerializer.Deserialize<ServerDetectionSignature>(File.ReadAllText(path), _jsonOptions);
                if (sig is not null)
                    signatures.Add(sig);
            }
            catch (Exception)
            {
                // Skip malformed or unreadable signature files; continue loading the rest.
            }
        }

        return signatures;
    }
}
