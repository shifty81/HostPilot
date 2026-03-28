# HostPilot / SteamServerTool — Full Mod Intake + Local Import Spec Scaffold

This pack adds a **managed local mod intake path** for drag-and-drop files, zip archives, and folders.

## What is included

### Core models
- `ModIntakeRules`
- `ModImportRequest`
- `ModImportCandidate`
- `ModInstallPlan`
- `InstalledModRecord`
- enum set for source type, package type, install action, and compatibility

### Core services
- `LocalModIntakeAnalyzer`
- `LocalModInstallPlanner`
- `LocalModImportService`
- `InstalledModRegistryService`
- `IModMetadataReader` extension point
- `HashUtility`

### UI scaffold
- `ModDropZoneControl.xaml`
- `ModDropZoneControl.xaml.cs`
- `ModImportItemViewModel.cs`

### Tests
- `LocalModImportServiceTests.cs`

## Intended flow

```text
Drop file/folder/zip
→ create ModImportRequest from selected server template rules
→ preview with LocalModImportService.PreviewAsync(...)
→ show review tray in Mods tab
→ execute with LocalModImportService.ExecuteAsync(...)
→ update installed mods registry
→ refresh installed list / restart badge
```

## Template examples

### Minecraft Java
```json
{
  "acceptFiles": [".jar", ".zip"],
  "acceptFolders": false,
  "autoExtractZip": false,
  "targetPath": "mods",
  "allowRawInstall": false,
  "requiresRestart": true
}
```

### Vintage Story
```json
{
  "acceptFiles": [".zip"],
  "acceptFolders": true,
  "autoExtractZip": false,
  "targetPath": "Mods",
  "allowRawInstall": true,
  "requiresRestart": true
}
```

### Generic plugin server
```json
{
  "acceptFiles": [".zip", ".dll", ".json"],
  "acceptFolders": true,
  "autoExtractZip": true,
  "targetPath": "Plugins",
  "configTargetPath": "Config",
  "allowRawInstall": true,
  "requiresRestart": true
}
```

## Integration notes

1. Add a mod-intake rules object to each deployment manifest or server template.
2. On Mods tab drag-drop, pass dropped path + selected server rules into `PreviewAsync`.
3. Render `ModInstallPlan.Items` in a review panel before copying anything.
4. Persist the installed-mod registry per server, for example:
   - `Servers/<server>/hostpilot.mods.json`
5. Later, plug Workshop / CurseForge / Vintage Story browser installs into the same registry.

## Important caveats

- This is scaffold code, not a fully wired final feature.
- Zip extraction currently assumes the archive should unpack to the destination folder selected by the plan.
- No malware scanning or digital signature validation is included yet.
- The UI control is not yet inserted into `MainWindow.xaml` or a dedicated Mods panel.
- I did not verify a live `dotnet build` in this environment, so I am not claiming a successful compile here.

## Recommended next pass

- add `ModIntakeRules` onto `ServerTemplate` and `ServerConfig`
- wire drag-drop from the real Mods tab / server popup
- add metadata readers for Minecraft jars and Vintage Story packages
- add duplicate detection against the registry
- add uninstall / enable-disable actions from the registry
- unify provider-installed mods and local imports under one installed list
