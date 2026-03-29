namespace HostPilot.Core.Providers.Models;

public sealed class ProviderDeploymentProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ProviderId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string InstallDirectory { get; set; } = "";
    public string ExecutablePath { get; set; } = "";
    public string WorkingDirectory { get; set; } = "";
    public string Host { get; set; } = "127.0.0.1";
    public int GamePort { get; set; }
    public int QueryPort { get; set; }
    public int RconPort { get; set; }
    public string? RconPassword { get; set; }
    public int StopTimeoutSeconds { get; set; } = 30;
    public Dictionary<string, object?> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
