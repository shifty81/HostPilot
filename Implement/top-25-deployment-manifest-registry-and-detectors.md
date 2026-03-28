
# Top-25 Deployment Manifest Registry + Actual Server Detectors Expansion

This pack adds a manifest-backed deployment registry and a matching signature-based detector layer so the deploy wizard and the first-run import flow can share the same source of truth.

## Added in this pass

- `SteamServerTool.Core/Models/DeploymentManifest.cs`
  - JSON model for exact deployment templates
- `SteamServerTool.Core/Services/Deployment/DeploymentManifestRegistry.cs`
  - loads all manifest JSON files from the registry folder
- `SteamServerTool.Core/Services/Deployment/ManifestTemplateMapper.cs`
  - bridges manifest data into the existing `ServerTemplate` shape
- `SteamServerTool.Core/Services/Discovery/ServerDetectionSignature.cs`
  - detector signature model
- `SteamServerTool.Core/Services/Discovery/SignatureRegistry.cs`
  - loads all detector signatures
- `SteamServerTool.Core/Services/Discovery/ManifestBackedServerDetector.cs`
  - scans candidate roots and returns detected installs with evidence/confidence
- `SteamServerTool.Core/DeploymentManifests/*.json`
  - 28 starter manifests: top-25 style dedicated server targets plus Minecraft Java, Minecraft Bedrock, and Vintage Story
- `SteamServerTool.Core/DiscoverySignatures/*.json`
  - matching detector signatures for those manifests
- tests covering manifest loading and detector matching

## What the registry now covers

Steam-oriented and custom templates included in this scaffold:

1. Palworld
2. Valheim
3. 7 Days to Die
4. ARK: Survival Ascended
5. ARK: Survival Evolved
6. Counter-Strike 2
7. Rust
8. Project Zomboid
9. Enshrouded
10. V Rising
11. Conan Exiles
12. Satisfactory
13. Space Engineers
14. Don't Starve Together
15. Team Fortress 2
16. Garry's Mod
17. Unturned
18. Terraria
19. tModLoader
20. Factorio
21. Core Keeper
22. Soulmask
23. Abiotic Factor
24. Icarus Dedicated Server
25. Sons of the Forest
26. Minecraft Java
27. Minecraft Bedrock
28. Vintage Story

## How it is meant to be used

### Deploy wizard

- load the manifest registry
- render template choices from manifests instead of only hardcoded templates
- prepopulate config fields from `fields`, `ports`, `configFiles`, and `cluster`

### First-run import

- load the signature registry
- scan known roots
- map discoveries back to manifest IDs
- let the user import the server as an external/managed entry

## Next recommended wiring

1. replace or supplement `ServerTemplates.All` with a manifest-driven loader
2. feed detector results into the existing import dialog
3. use manifest fields to auto-build exact config pages per server type
4. add a profile image/icon and richer UI metadata later

## Important note

This environment does not have the .NET SDK installed, so this pack was not compile-verified here.
