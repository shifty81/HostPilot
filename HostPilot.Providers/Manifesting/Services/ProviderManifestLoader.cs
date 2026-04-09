using System.Text.Json;
using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public sealed class ProviderManifestLoader : IProviderManifestLoader
{
    private readonly IProviderManifestRegistry _registry;
    private readonly IManifestSchemaValidator _schemaValidator;
    private readonly IManifestInheritanceResolver _inheritanceResolver;
    private readonly string _manifestRoot;

    public ProviderManifestLoader(
        IProviderManifestRegistry registry,
        IManifestSchemaValidator schemaValidator,
        IManifestInheritanceResolver inheritanceResolver,
        string manifestRoot)
    {
        _registry = registry;
        _schemaValidator = schemaValidator;
        _inheritanceResolver = inheritanceResolver;
        _manifestRoot = manifestRoot;
    }

    public async Task<ManifestLoadResult> LoadAsync(string manifestId, CancellationToken cancellationToken = default)
    {
        var entry = await _registry.FindByIdAsync(manifestId, cancellationToken)
                    ?? throw new InvalidOperationException($"Manifest '{manifestId}' was not found in the registry.");

        var fullPath = Path.Combine(_manifestRoot, entry.ManifestPath.Replace('/', Path.DirectorySeparatorChar));
        var json = await File.ReadAllTextAsync(fullPath, cancellationToken);
        await _schemaValidator.ValidateAsync(json, cancellationToken);

        var manifest = JsonSerializer.Deserialize<ProviderManifest>(json, JsonManifestSerializer.Options)
                       ?? throw new InvalidOperationException($"Failed to deserialize manifest '{manifestId}'.");

        var resolved = await _inheritanceResolver.ResolveAsync(manifest, cancellationToken);
        VerifyReferences(resolved);

        return new ManifestLoadResult
        {
            Manifest = resolved
        };
    }

    private static void VerifyReferences(ProviderManifest manifest)
    {
        var tabIds = manifest.Tabs.Select(x => x.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var sectionIds = manifest.Sections.Select(x => x.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var section in manifest.Sections)
        {
            if (!tabIds.Contains(section.TabId))
            {
                throw new InvalidOperationException($"Section '{section.Id}' references unknown tab '{section.TabId}'.");
            }
        }

        foreach (var field in manifest.Fields)
        {
            if (!sectionIds.Contains(field.SectionId))
            {
                throw new InvalidOperationException($"Field '{field.Id}' references unknown section '{field.SectionId}'.");
            }
        }
    }
}
