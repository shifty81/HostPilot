using HostPilot.Providers.Manifesting.Contracts;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ManifestInheritanceResolver : IManifestInheritanceResolver
{
    private readonly string _manifestRoot;
    private readonly Lazy<Dictionary<string, string>> _pathCache;

    public ManifestInheritanceResolver(string manifestRoot)
    {
        _manifestRoot = manifestRoot;
        _pathCache = new Lazy<Dictionary<string, string>>(BuildPathCache, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<ProviderManifest> ResolveAsync(ProviderManifest manifest, CancellationToken cancellationToken = default)
    {
        if (manifest.Inherits.Count == 0)
        {
            return manifest;
        }

        var resolved = manifest;
        foreach (var inheritedId in manifest.Inherits)
        {
            var basePath = FindManifestPathById(inheritedId);
            var baseJson = await File.ReadAllTextAsync(basePath, cancellationToken);
            var baseManifest = System.Text.Json.JsonSerializer.Deserialize<ProviderManifest>(baseJson, JsonManifestSerializer.Options)
                               ?? throw new InvalidOperationException($"Failed to deserialize inherited manifest '{inheritedId}'.");
            resolved = ManifestMergeUtility.Merge(baseManifest, resolved);
        }

        return resolved;
    }

    private Dictionary<string, string> BuildPathCache()
    {
        var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(_manifestRoot))
        {
            return cache;
        }

        foreach (var file in Directory.EnumerateFiles(_manifestRoot, "*.json", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);
            var idStart = text.IndexOf("\"id\":", StringComparison.OrdinalIgnoreCase);
            if (idStart < 0) continue;
            var valueStart = text.IndexOf('"', idStart + 5);
            if (valueStart < 0) continue;
            var valueEnd = text.IndexOf('"', valueStart + 1);
            if (valueEnd < 0) continue;
            var id = text[(valueStart + 1)..valueEnd];
            if (!string.IsNullOrWhiteSpace(id) && !cache.ContainsKey(id))
            {
                cache[id] = file;
            }
        }

        return cache;
    }

    private string FindManifestPathById(string manifestId)
    {
        if (_pathCache.Value.TryGetValue(manifestId, out var path))
        {
            return path;
        }

        throw new FileNotFoundException($"Could not locate inherited manifest '{manifestId}'.");
    }
}

internal static class ManifestMergeUtility
{
    public static ProviderManifest Merge(ProviderManifest parent, ProviderManifest child)
    {
        return new ProviderManifest
        {
            Id = child.Id,
            Version = child.Version,
            SchemaVersion = child.SchemaVersion,
            DisplayName = child.DisplayName,
            Description = child.Description ?? parent.Description,
            ProviderType = child.ProviderType,
            GameFamily = child.GameFamily,
            Metadata = child.Metadata,
            Inherits = child.Inherits,
            Capabilities = MergeCapabilities(parent.Capabilities, child.Capabilities),
            Deployment = MergeDeployment(parent.Deployment, child.Deployment),
            Tabs = MergeById(parent.Tabs, child.Tabs, x => x.Id),
            Sections = MergeById(parent.Sections, child.Sections, x => x.Id),
            Fields = MergeById(parent.Fields, child.Fields, x => x.Id),
            Presets = [.. parent.Presets, .. child.Presets],
            CrossFieldValidations = [.. parent.CrossFieldValidations, .. child.CrossFieldValidations],
            Defaults = MergeDictionary(parent.Defaults, child.Defaults),
            Operations = MergeDictionary(parent.Operations, child.Operations),
            Integration = child.Integration,
            Detection = child.Detection,
            Advanced = child.Advanced
        };
    }

    private static ProviderCapabilitySet MergeCapabilities(ProviderCapabilitySet parent, ProviderCapabilitySet child) => new()
    {
        SupportsInstall = child.SupportsInstall || parent.SupportsInstall,
        SupportsUpdate = child.SupportsUpdate || parent.SupportsUpdate,
        SupportsImport = child.SupportsImport || parent.SupportsImport,
        SupportsStart = child.SupportsStart || parent.SupportsStart,
        SupportsStop = child.SupportsStop || parent.SupportsStop,
        SupportsValidation = child.SupportsValidation || parent.SupportsValidation,
        SupportsRcon = child.SupportsRcon || parent.SupportsRcon,
        SupportsQuery = child.SupportsQuery || parent.SupportsQuery,
        SupportsMods = child.SupportsMods || parent.SupportsMods,
        SupportsWorkshop = child.SupportsWorkshop || parent.SupportsWorkshop,
        SupportsCluster = child.SupportsCluster || parent.SupportsCluster,
        SupportsBackups = child.SupportsBackups || parent.SupportsBackups,
        SupportsProfiles = child.SupportsProfiles || parent.SupportsProfiles,
        SupportsConfigFiles = child.SupportsConfigFiles || parent.SupportsConfigFiles,
        SupportsCustomStartArgs = child.SupportsCustomStartArgs || parent.SupportsCustomStartArgs,
        SupportsMultipleInstances = child.SupportsMultipleInstances || parent.SupportsMultipleInstances,
        SupportsSaveDiscovery = child.SupportsSaveDiscovery || parent.SupportsSaveDiscovery
    };

    private static DeploymentDefinition MergeDeployment(DeploymentDefinition parent, DeploymentDefinition child) => new()
    {
        InstallStrategy = child.InstallStrategy ?? parent.InstallStrategy,
        SteamAppId = child.SteamAppId ?? parent.SteamAppId,
        AnonymousLogin = child.AnonymousLogin,
        DownloadSource = child.DownloadSource ?? parent.DownloadSource,
        DefaultInstallSubpath = child.DefaultInstallSubpath ?? parent.DefaultInstallSubpath,
        DefaultExecutable = child.DefaultExecutable ?? parent.DefaultExecutable,
        WorkingDirectory = child.WorkingDirectory ?? parent.WorkingDirectory,
        StartupMode = child.StartupMode ?? parent.StartupMode,
        ConfigStorageMode = child.ConfigStorageMode ?? parent.ConfigStorageMode,
        RuntimeDependencyIds = child.RuntimeDependencyIds.Count > 0 ? child.RuntimeDependencyIds : parent.RuntimeDependencyIds
    };

    private static List<T> MergeById<T>(IEnumerable<T> parent, IEnumerable<T> child, Func<T, string> idSelector)
    {
        var result = parent.ToDictionary(idSelector, x => x, StringComparer.OrdinalIgnoreCase);
        foreach (var item in child)
        {
            result[idSelector(item)] = item;
        }

        return result.Values.ToList();
    }

    private static Dictionary<string, TValue> MergeDictionary<TValue>(
        IReadOnlyDictionary<string, TValue> parent,
        IReadOnlyDictionary<string, TValue> child)
    {
        var output = new Dictionary<string, TValue>(parent, StringComparer.OrdinalIgnoreCase);
        foreach (var pair in child)
        {
            output[pair.Key] = pair.Value;
        }

        return output;
    }
}
