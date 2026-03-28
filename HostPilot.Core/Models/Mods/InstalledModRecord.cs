using System.Text.Json.Serialization;

namespace HostPilot.Core.Models.Mods;

public class InstalledModRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("serverName")]
    public string ServerName { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = "local";

    [JsonPropertyName("provider")]
    public string? Provider { get; set; }

    [JsonPropertyName("sourcePath")]
    public string SourcePath { get; set; } = "";

    [JsonPropertyName("installedPath")]
    public string InstalledPath { get; set; } = "";

    [JsonPropertyName("hashSha256")]
    public string? HashSha256 { get; set; }

    [JsonPropertyName("installedAtUtc")]
    public DateTime InstalledAtUtc { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("requiresRestart")]
    public bool RequiresRestart { get; set; }

    [JsonPropertyName("managed")]
    public bool Managed { get; set; } = true;
}
