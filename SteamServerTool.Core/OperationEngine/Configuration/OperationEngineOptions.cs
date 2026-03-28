namespace SteamServerTool.Core.OperationEngine.Configuration;

public sealed class OperationEngineOptions
{
    public int MaxConcurrentOperations { get; set; } = 2;
    public int PollingDelayMilliseconds { get; set; } = 150;
}
