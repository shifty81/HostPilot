namespace HostPilot.Core.Execution.Contracts;

public sealed record NodeDescriptor(
    string NodeId,
    string DisplayName,
    string Region,
    bool IsOnline,
    int AvailableCpuCores,
    int AvailableMemoryMb,
    int AvailableStorageMb,
    bool HasSteamCmd,
    bool SupportsRcon,
    IReadOnlyList<string> Capabilities,
    IReadOnlyList<string> Tags,
    double HealthScore,
    int RunningWorkItemCount);
