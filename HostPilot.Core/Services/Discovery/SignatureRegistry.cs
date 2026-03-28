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

        return Directory.EnumerateFiles(_directory, "*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName)
            .Select(path => JsonSerializer.Deserialize<ServerDetectionSignature>(File.ReadAllText(path), _jsonOptions))
            .Where(sig => sig is not null)
            .Cast<ServerDetectionSignature>()
            .ToList();
    }
}
