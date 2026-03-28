using System;
using System.Collections.Generic;

namespace HostPilot.Core.Models.Mods;

public sealed class ModCatalogItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Version { get; set; }
    public string? Author { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? DownloadUrl { get; set; }
    public long? SizeBytes { get; set; }
    public DateTimeOffset? UpdatedUtc { get; set; }
    public bool HasDependencies { get; set; }
    public bool SupportsServerSideInstall { get; set; } = true;
    public ModProviderType ProviderType { get; set; }
    public List<string> DependencyIds { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
