using HostPilot.Core.Models;

namespace HostPilot.Core.Services.Deployment;

public class DeploymentTemplateCatalog
{
    private readonly DeploymentManifestRegistry _registry;
    private IReadOnlyList<ServerTemplate>? _cached;

    public DeploymentTemplateCatalog(DeploymentManifestRegistry registry)
    {
        _registry = registry;
    }

    public IReadOnlyList<ServerTemplate> GetTemplates(bool includeLegacyFallback = true)
    {
        if (_cached is not null)
            return _cached;

        var manifestTemplates = _registry.LoadAll()
            .Select(ManifestTemplateMapper.ToTemplate)
            .Where(t => !string.IsNullOrWhiteSpace(t.Name))
            .ToList();

        if (manifestTemplates.Count == 0 && includeLegacyFallback)
            manifestTemplates = ServerTemplates.All.ToList();

        _cached = manifestTemplates;
        return _cached;
    }

    public ServerTemplate? FindByManifestId(string manifestId)
    {
        var manifest = _registry.LoadById(manifestId);
        return manifest is null ? null : ManifestTemplateMapper.ToTemplate(manifest);
    }
}
