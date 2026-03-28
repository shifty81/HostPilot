using System.Text.Json.Serialization;

namespace HostPilot.Core.Models;

public class DeploymentManifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "🎮";

    [JsonPropertyName("tier")]
    public string Tier { get; set; } = "A";

    [JsonPropertyName("distribution")]
    public string Distribution { get; set; } = "steamcmd";

    [JsonPropertyName("appId")]
    public int? AppId { get; set; }

    [JsonPropertyName("defaultDir")]
    public string DefaultDir { get; set; } = "";

    [JsonPropertyName("executable")]
    public string Executable { get; set; } = "";

    [JsonPropertyName("launchArgsTemplate")]
    public string LaunchArgsTemplate { get; set; } = "";

    [JsonPropertyName("configFiles")]
    public List<string> ConfigFiles { get; set; } = new();

    [JsonPropertyName("ports")]
    public List<ManifestPort> Ports { get; set; } = new();

    [JsonPropertyName("fields")]
    public List<ManifestField> Fields { get; set; } = new();

    [JsonPropertyName("cluster")]
    public ManifestClusterSpec? Cluster { get; set; }

    [JsonPropertyName("discovery")]
    public ManifestDiscoverySpec Discovery { get; set; } = new();

    [JsonPropertyName("notes")]
    public List<string> Notes { get; set; } = new();
}

public class ManifestPort
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("default")]
    public int Default { get; set; }

    [JsonPropertyName("protocol")]
    public string Protocol { get; set; } = "udp";
}

public class ManifestField
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

    [JsonPropertyName("default")]
    public string Default { get; set; } = "";

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}

public class ManifestClusterSpec
{
    [JsonPropertyName("supported")]
    public bool Supported { get; set; }

    [JsonPropertyName("sharedFields")]
    public List<string> SharedFields { get; set; } = new();

    [JsonPropertyName("perNodeFields")]
    public List<string> PerNodeFields { get; set; } = new();
}

public class ManifestDiscoverySpec
{
    [JsonPropertyName("executables")]
    public List<string> Executables { get; set; } = new();

    [JsonPropertyName("files")]
    public List<string> Files { get; set; } = new();

    [JsonPropertyName("folders")]
    public List<string> Folders { get; set; } = new();

    [JsonPropertyName("confidence")]
    public double Confidence { get; set; } = 0.75;
}
