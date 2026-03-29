namespace HostPilot.Core.Providers.Models;

public sealed class RconCommandPlan
{
    public string CommandText { get; init; } = "";
    public bool WaitForEmptyResponseOk { get; init; }
}
