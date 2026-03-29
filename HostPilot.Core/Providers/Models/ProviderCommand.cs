namespace HostPilot.Core.Providers.Models;

public sealed class ProviderCommand
{
    public string Executable { get; init; } = "";
    public string Arguments { get; init; } = "";
    public string WorkingDirectory { get; init; } = "";
    public bool RequiresSteamCmd { get; init; }
    public bool RequiresRcon { get; init; }
    public IReadOnlyDictionary<string, string> Environment { get; init; } = new Dictionary<string, string>();

    public override string ToString() => $"\"{Executable}\" {Arguments}".Trim();
}
