# Mod Provider Wiring Pack

This pass moves the mod system from placeholder-only scaffolding toward host-level wiring.

## What changed
- Added a `RelayCommand` utility so the Mods tab can use command-driven actions.
- Expanded `ModsTabViewModel` to hold full server context, active profile summary, installed-mod list, and install actions.
- Added `ManagedModInstallCoordinator` to normalize catalog installs into a managed target path and to reflect existing workshop IDs plus detected local files.
- Added HTTP-provider-ready scaffolding:
  - `CurseForgeHttpProvider`
  - `VintageStoryHttpProvider`
  - shared `JsonModProviderBase`
  - configurable endpoint options model
- Wired `MainWindow` to:
  - build a provider registry
  - create the Mods tab view model
  - resolve a server template from the selected server config
  - push the selected server into the Mods tab whenever the user changes selection
- Replaced the legacy hardcoded workshop-only Mods tab surface with the embedded `ModsTabView` control.

## Important notes
- The external provider URLs in this scaffold deliberately use placeholder endpoints (`example.invalid`).
- That keeps this pack safe to drop into the repo without silently binding to a production API contract before you finish the exact provider research and auth flow.
- `ManagedModInstallCoordinator` writes marker files for non-workshop installs so the UI has a real managed install path to display even before the final downloader/extractor logic is connected.

## What this pack enables
- Game-aware Mods tab driven by the selected server/template
- Browser search + install command flow
- Installed/configured mods panel
- A host-side integration point for real CurseForge / Vintage Story / other ecosystem clients

## Next step after this pack
- Replace placeholder endpoints with researched provider settings
- Add authenticated HTTP clients where needed
- Connect browser installs and local imports into the real operation queue / install pipeline
- Persist managed mod registry entries beyond the current config-backed + filesystem-detected model
