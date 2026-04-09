namespace HostPilot.Core.Models.Templates;

public sealed class DeploymentTemplateBundle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Slug { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string Author { get; set; } = string.Empty;
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public List<string> Tags { get; set; } = new();
    public Dictionary<string, string> ConfigValues { get; set; } = new();
    public List<TemplateModReference> Mods { get; set; } = new();
    public string? ThumbnailPath { get; set; }
    public string? ReadmeMarkdown { get; set; }
    public TemplateSignature? Signature { get; set; }
}
