using HostPilot.Providers.Manifesting.Enums;

namespace HostPilot.Providers.Manifesting.Contracts;

public sealed class ManifestOperationDefinition
{
    public required ManifestOperationType Type { get; init; }
    public int? AppId { get; init; }
    public string? PathKey { get; init; }
    public string? WorkingDirectoryKey { get; init; }
    public string? ArgumentsBuilder { get; init; }
    public string? RconCommand { get; init; }
}
