# Installed Server Discovery + Provider Settings + Real Execution Pack

This pack adds scaffold files for three major systems:

1. **First-run installed server discovery**
2. **Provider settings and secret storage**
3. **Real provider execution bridge for mod installs and dependency review**

## What this pack is meant to solve

- scan the PC on first run for likely existing dedicated server installs
- prompt the user to import them instead of reinstalling them
- store provider configuration cleanly, including API keys
- route provider installs through the operation engine instead of directly mutating files from the UI
- present a dependency review layer before mod install execution for CurseForge and Vintage Story style providers

## Included scaffold areas

### Discovery
- `InstalledServerDiscoveryService`
- `IServerFootprintDetector`
- `SteamLibraryLocator`
- `DiscoveryRootLocator`
- `ServerImportCoordinator`
- sample detectors for SteamCMD, Minecraft Java, and Vintage Story footprints

### Provider settings
- `ProviderSettings`
- `IProviderSettingsStore`
- `JsonProviderSettingsStore`
- `ISecretStore`
- `FileSecretStore`
- provider settings WPF control + view model

### Real execution bridge
- `IModOperationDispatcher`
- `ProviderInstallExecutionBridge`
- `DependencyReviewService`
- `DependencyReviewViewModel`
- `ImportDiscoveredServersDialog`

## Important notes

- This pack is a scaffold. It is intentionally conservative and uses interfaces where the repo currently has direct service calls.
- I did **not** verify a live .NET compile in this environment.
- The discovery system defaults to targeted roots rather than a whole-disk crawl.
- Secrets are stored in a local JSON-backed stub in this pack. In production, replace that with a platform-protected storage mechanism.

## Suggested integration order

1. Run discovery on first-run dialog completion.
2. Show import candidates dialog.
3. Save imported servers as externally managed entries.
4. Add provider settings page to application settings.
5. Inject settings + secret stores into mod providers.
6. Route browser installs through `ProviderInstallExecutionBridge`.
7. Add dependency review dialog before queueing install operations.

## Recommended later upgrades

- Windows Credential Manager or DPAPI-backed secret storage
- Steam library VDF parsing instead of plain-folder heuristics only
- more detectors for your top 25 template roster
- cluster-aware import candidate grouping
- richer dependency conflict review
