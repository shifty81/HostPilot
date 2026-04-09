using System.Text.Json;
using System.Text.Json.Serialization;

namespace HostPilot.Providers.Manifesting.Services;

public static class JsonManifestSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };
}
