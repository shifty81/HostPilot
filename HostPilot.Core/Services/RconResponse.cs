namespace HostPilot.Core.Services;

public sealed class RconResponse
{
    public bool Succeeded { get; init; }
    public string ResponseText { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
}
