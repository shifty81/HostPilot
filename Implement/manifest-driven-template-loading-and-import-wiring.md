# Manifest-Driven Template Loading + Detector Import Wiring Pack

This pack moves the UI off the hardcoded `ServerTemplates.All` path and onto the manifest-backed template registry added in the previous pack.

## What changed

- Added `DeploymentTemplateCatalog` to load `ServerTemplate` objects from deployment manifests with a built-in fallback path.
- Updated `MainWindow` to use manifest-loaded templates for the main template list and new-server wizard.
- Added `DiscoveredServerImportFactory` so detector hits can be turned into real `ServerConfig` entries.
- Added `ImportDiscoveredServersDialog` so first launch can present discovered installs and let the user import selected ones.
- Wired startup flow to:
  1. ensure SteamCMD setup is handled,
  2. run manifest-backed detector scans on common roots,
  3. prompt for import,
  4. persist a first-run completion marker.

## Discovery roots

This scaffold scans targeted locations instead of crawling the full disk:

- local `Servers/`
- common Steam library roots under Program Files / Program Files (x86)
- common `SteamCMD` data path
- already-known managed server directories

## Notes

- The import flow is intentionally non-destructive. Imported servers are added as managed entries but their existing files are not overwritten.
- Imported entries are tagged with `Imported` and include captured detector evidence in the notes field.
- This pack still needs a live `.NET` build verification outside this environment.
