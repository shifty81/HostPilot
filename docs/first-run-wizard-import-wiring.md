# First-Run Wizard Integration + Actual Import Workflow Wiring

This pass wires the previously scaffolded discovery system into the desktop startup flow so the application can:

1. prompt for SteamCMD on first run,
2. scan the machine for already-installed dedicated servers,
3. present an import review dialog,
4. add selected installs into `servers.json` as imported/external-managed entries, and
5. persist first-run completion state so the sequence does not rerun every launch.

## Added pieces

### Core
- `Models/FirstRun/FirstRunWizardState.cs`
- `Services/Startup/IFirstRunStateStore.cs`
- `Services/Startup/JsonFirstRunStateStore.cs`
- `Services/Startup/FirstRunWizardCoordinator.cs`
- fixed `Services/Discovery/ServerImportCoordinator.cs` to map into the current `ServerConfig` schema

### UI
- `ImportDiscoveredServersViewModel` now supports per-candidate checkbox selection
- `ImportDiscoveredServersDialog` now exposes a real import-selection grid
- `MainWindow` now runs a startup sequence after first paint instead of only checking SteamCMD

### Tests
- `ServerImportCoordinatorTests.cs`
- `JsonFirstRunStateStoreTests.cs`

## Startup flow

```text
MainWindow Loaded
→ load first-run state
→ if SteamCMD missing, show FirstRunSetupDialog
→ if discovery not yet completed, run InstalledServerDiscoveryService
→ show ImportDiscoveredServersDialog
→ import checked candidates into ServerManager + save servers.json
→ save first-run completion state
```

## Persistence

The first-run state is stored at:

```text
settings/first-run-state.json
```

Tracked flags:
- `HasCompletedWizard`
- `HasConfiguredSteamCmd`
- `HasCompletedDiscoveryScan`
- `LastCompletedAtUtc`
- `LastDiscoveryScanAtUtc`
- `ImportedCandidateIds`

## Import behavior

Imported servers are added non-destructively:
- existing install path is reused
- executable is mapped relative to the install directory where possible
- group/tag metadata mark the entry as imported
- no reinstall occurs
- no existing files are overwritten by this wiring pass

## Notes

This is still a scaffold/integration pass. It does **not** claim build verification in-container because the .NET SDK is unavailable here.

Recommended next move:
- add a full-screen first-run wizard shell with progress states,
- allow custom folders during discovery scan,
- add duplicate-name resolution during import,
- enrich detector coverage for the broader server template library.
