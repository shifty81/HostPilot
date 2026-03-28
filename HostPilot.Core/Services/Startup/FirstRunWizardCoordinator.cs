using HostPilot.Core.Models;
using HostPilot.Core.Models.Discovery;
using HostPilot.Core.Models.FirstRun;
using HostPilot.Core.Services.Discovery;

namespace HostPilot.Core.Services.Startup;

public sealed class FirstRunWizardCoordinator
{
    private readonly SteamCmdService _steamCmdService;
    private readonly InstalledServerDiscoveryService _discoveryService;
    private readonly ServerImportCoordinator _importCoordinator;
    private readonly ServerManager _serverManager;
    private readonly IFirstRunStateStore _stateStore;
    private readonly string _configPath;

    public FirstRunWizardCoordinator(
        SteamCmdService steamCmdService,
        InstalledServerDiscoveryService discoveryService,
        ServerImportCoordinator importCoordinator,
        ServerManager serverManager,
        IFirstRunStateStore stateStore,
        string configPath)
    {
        _steamCmdService = steamCmdService;
        _discoveryService = discoveryService;
        _importCoordinator = importCoordinator;
        _serverManager = serverManager;
        _stateStore = stateStore;
        _configPath = configPath;
    }

    public FirstRunWizardState LoadState() => _stateStore.Load();

    public bool ShouldRunWizard()
    {
        var state = _stateStore.Load();
        return !state.HasCompletedWizard || !_steamCmdService.IsSteamCmdInstalled();
    }


    public IReadOnlyList<ServerConfig> ImportSelectedCandidates(IEnumerable<DiscoveredServerCandidate> candidates)
    {
        var existingPaths = new HashSet<string>(
            _serverManager.Servers.Select(x => x.Dir),
            StringComparer.OrdinalIgnoreCase);

        var imported = new List<ServerConfig>();
        foreach (var candidate in candidates.Where(c => !c.IsImported))
        {
            if (existingPaths.Contains(candidate.InstallPath))
            {
                continue;
            }

            var config = _importCoordinator.BuildImportedConfig(candidate);
            _serverManager.Servers.Add(config);
            existingPaths.Add(config.Dir);
            candidate.IsImported = true;
            imported.Add(config);
        }

        if (imported.Count > 0)
        {
            _serverManager.SaveConfig(_configPath);
        }

        var state = _stateStore.Load();
        state.HasCompletedDiscoveryScan = true;
        state.LastDiscoveryScanAtUtc = DateTimeOffset.UtcNow;
        foreach (var importedCandidate in candidates.Where(c => c.IsImported))
        {
            if (!state.ImportedCandidateIds.Any(x => string.Equals(x, importedCandidate.CandidateId, StringComparison.OrdinalIgnoreCase)))
            {
                state.ImportedCandidateIds.Add(importedCandidate.CandidateId);
            }
        }

        _stateStore.Save(state);
        return imported;
    }

    public void CompleteWizard(bool steamCmdConfigured, bool discoveryWasShown)
    {
        var state = _stateStore.Load();
        state.HasConfiguredSteamCmd = steamCmdConfigured;
        state.HasCompletedDiscoveryScan = state.HasCompletedDiscoveryScan || discoveryWasShown;
        state.HasCompletedWizard = true;
        state.LastCompletedAtUtc = DateTimeOffset.UtcNow;
        _stateStore.Save(state);
    }
}
