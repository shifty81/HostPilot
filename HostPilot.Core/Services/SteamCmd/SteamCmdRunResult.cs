namespace HostPilot.Core.Services.SteamCmd;

public sealed class SteamCmdRunResult
{
    public bool Succeeded { get; init; }
    public string Summary { get; init; } = string.Empty;

    public static SteamCmdRunResult Success(string summary = "Completed successfully.") =>
        new() { Succeeded = true, Summary = summary };

    public static SteamCmdRunResult Failure(string summary) =>
        new() { Succeeded = false, Summary = summary };
}
