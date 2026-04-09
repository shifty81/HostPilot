namespace HostPilot.Core.Models.Templates;

public sealed class TemplateReviewResult
{
    public bool CanApply { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Notices { get; set; } = new();
}
