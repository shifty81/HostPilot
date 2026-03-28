# Mod Browser + Template Integration Pack

This pack is the next pass after local mod intake.

## Goals
- unify workshop/API/browser mods with local drag-drop imports
- make the Mods tab game-aware from the selected server template
- support Steam Workshop, CurseForge-style, Vintage Story site index, and local/manual imports
- route all installs through the operation engine so installs are queued, tracked, and recoverable

## Added scaffold areas
- `SteamServerTool.Core/Models/Mods/*`
- `SteamServerTool.Core/Services/Mods/*`
- `SteamServerTool/ViewModels/ModsTabViewModel.cs`
- `SteamServerTool/Controls/ModsTabView.xaml`
- `SteamServerTool/Dialogs/ModImportReviewDialog.*`
- `SteamServerTool.Tests/Mods/*`

## Intended flow
1. Load selected server and template.
2. Resolve supported mod provider(s) from template.
3. Load installed mods from registry.
4. Browse/search provider catalog if supported.
5. Accept drag-drop local content.
6. Build install plan.
7. Queue `INSTALL_MOD` or `INSTALL_LOCAL_MOD` through the operation engine.
8. Refresh installed list, warnings, and restart-needed state.

## Template-driven provider rules
Each server template should eventually carry a `ModSupportProfile` with:
- provider type
- accepted local file types
- local install root
- browser visibility
- dependency support
- loader compatibility requirements
- restart requirement policy

## Recommended next implementation pass
- wire `ModsTabViewModel` into the actual selected-server screen
- bind template lookup from your existing `ServerTemplate`
- connect queued operations to the operation engine introduced earlier
- replace placeholder provider clients with real web/API adapters per game ecosystem
