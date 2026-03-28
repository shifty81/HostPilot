using System.Text.Json.Serialization;

namespace HostPilot.Core.Models.Mods;

public class ModIntakeRules
{
    [JsonPropertyName("acceptFiles")]
    public List<string> AcceptFiles { get; set; } = new();

    [JsonPropertyName("acceptFolders")]
    public bool AcceptFolders { get; set; }

    [JsonPropertyName("autoExtractZip")]
    public bool AutoExtractZip { get; set; }

    [JsonPropertyName("targetPath")]
    public string TargetPath { get; set; } = "mods";

    [JsonPropertyName("configTargetPath")]
    public string ConfigTargetPath { get; set; } = "config";

    [JsonPropertyName("allowRawInstall")]
    public bool AllowRawInstall { get; set; }

    [JsonPropertyName("requiresRestart")]
    public bool RequiresRestart { get; set; } = true;

    [JsonPropertyName("allowMultipleModsPerFolder")]
    public bool AllowMultipleModsPerFolder { get; set; } = true;

    public bool AcceptsExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension)) return false;
        return AcceptFiles.Any(x => string.Equals(x, extension, StringComparison.OrdinalIgnoreCase));
    }
}
