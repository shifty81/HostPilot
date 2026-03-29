using HostPilot.Core.Providers.Models;

namespace HostPilot.Core.Providers.Abstractions;

public interface IGameProviderAdapter
{
    string Id { get; }
    string DisplayName { get; }
    string InstallChannel { get; }
    int? SteamAppId { get; }
    IProviderLogParser LogParser { get; }

    ProviderCommand BuildInstallCommand(ProviderDeploymentProfile profile, string steamCmdPath);
    ProviderCommand BuildUpdateCommand(ProviderDeploymentProfile profile, string steamCmdPath);
    ProviderCommand BuildValidateCommand(ProviderDeploymentProfile profile, string steamCmdPath);
    ProviderCommand BuildStartCommand(ProviderDeploymentProfile profile);
    IReadOnlyList<RconCommandPlan> BuildGracefulStopPlan(ProviderDeploymentProfile profile);
    bool CanUseRconStop(ProviderDeploymentProfile profile);
    string BuildHumanSummary(ProviderDeploymentProfile profile);
}
