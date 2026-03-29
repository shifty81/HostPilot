using System.Text.Json.Serialization;

namespace HostPilot.Core.Services.Discovery;

public class ServerDetectionSignature
{
    [JsonPropertyName("manifestId")]
    public string ManifestId { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("executables")]
    public List<string> Executables { get; set; } = new();

    [JsonPropertyName("files")]
    public List<string> Files { get; set; } = new();

    [JsonPropertyName("folders")]
    public List<string> Folders { get; set; } = new();

    [JsonPropertyName("configFiles")]
    public List<string> ConfigFiles { get; set; } = new();

    [JsonPropertyName("minimumEvidence")]
    public int MinimumEvidence { get; set; } = 2;
}
