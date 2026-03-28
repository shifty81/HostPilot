# Research-Backed Mod Provider Pack

This pass replaces the placeholder provider wiring with more realistic provider scaffolding built around the current documented capabilities of each ecosystem.

## Included

- `CurseForgeApiOptions`
- `CurseForgeModLoaderMapper`
- upgraded `CurseForgeHttpProvider`
  - documented `GET /v1/mods/search`
  - documented `GET /v1/mods/{id}`
  - documented file/dependency lookups via mod file endpoint
- upgraded `VintageStoryHttpProvider`
  - `GET /api/mods`
  - `GET /api/mod/{id}`
  - parsing of release download URLs and version tags
- new `SteamWorkshopApiProvider`
  - official `GetPublishedFileDetails` lookup by workshop id or workshop URL
  - intentionally limited to detail lookup rather than arbitrary public search
- `SteamWorkshopUrlParser`
- `SteamWorkshopInstallPlanner`
- tests for workshop id parsing, workshop download command formatting, and CurseForge loader mapping
- `MainWindow` provider registry updated to use the researched providers

## Important design choice

Steam Workshop is not treated the same way as CurseForge or Vintage Story.

For desktop-hosted game server management, the safe and supportable path is:

- browse/search where there is a stable documented API
- use direct workshop item URL / ID import for Steam Workshop
- convert that into a managed `steamcmd +workshop_download_item ...` install plan

This keeps the tool aligned with official/publicly documented Steam capabilities instead of depending on brittle page scraping.

## Current environment variable hook

CurseForge API key is pulled from:

- `HOSTPILOT_CURSEFORGE_API_KEY`

That keeps the scaffold from hardcoding secrets in source.

## Follow-up recommended next

1. Add app-id-aware Steam Workshop install UI for each template.
2. Add dedicated provider settings page for API keys and rate-limit diagnostics.
3. Add mod dependency follow-up resolution in the install review dialog.
4. Add optional local cache/index layer for workshop entries imported by URL so they appear in later searches/history.
