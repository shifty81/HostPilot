namespace HostPilot.Providers.Manifesting.Models;

public sealed class ManifestOutputMap
{
    public List<string> LaunchArguments { get; init; } = [];
    public Dictionary<string, Dictionary<string, object?>> CfgFiles { get; init; } = new();
    public Dictionary<string, Dictionary<string, object?>> JsonFiles { get; init; } = new();
    public Dictionary<string, string> EnvironmentVariables { get; init; } = new();
}
