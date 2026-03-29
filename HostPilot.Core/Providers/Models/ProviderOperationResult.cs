namespace HostPilot.Core.Providers.Models;

public sealed class ProviderOperationResult
{
    public bool Success { get; set; }
    public string Summary { get; set; } = "";
    public int? ExitCode { get; set; }
    public List<string> Notes { get; } = new();
}
