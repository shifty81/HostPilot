using HostPilot.Providers.Manifesting.Enums;

namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ProviderManifest
{
    public required string Id { get; init; }
    public required string Version { get; init; }
    public required string SchemaVersion { get; init; }
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
    public required ProviderType ProviderType { get; init; }
    public required string GameFamily { get; init; }
    public ProviderManifestMetadata Metadata { get; init; } = new();
    public List<string> Inherits { get; init; } = [];
    public ProviderCapabilitySet Capabilities { get; init; } = new();
    public DeploymentDefinition Deployment { get; init; } = new();
    public List<ManifestTabDefinition> Tabs { get; init; } = [];
    public List<ManifestSectionDefinition> Sections { get; init; } = [];
    public List<ManifestFieldDefinition> Fields { get; init; } = [];
    public List<ManifestPresetDefinition> Presets { get; init; } = [];
    public List<ManifestCrossFieldValidationDefinition> CrossFieldValidations { get; init; } = [];
    public Dictionary<string, object?> Defaults { get; init; } = new();
    public Dictionary<string, ManifestOperationDefinition> Operations { get; init; } = new();
    public ManifestIntegrationDefinition Integration { get; init; } = new();
    public ManifestDetectionDefinition Detection { get; init; } = new();
    public ManifestAdvancedDefinition Advanced { get; init; } = new();
}
