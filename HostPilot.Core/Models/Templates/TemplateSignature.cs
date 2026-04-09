namespace HostPilot.Core.Models.Templates;

public sealed class TemplateSignature
{
    public string Algorithm { get; set; } = "SHA256";
    public string Checksum { get; set; } = string.Empty;
    public string? Signer { get; set; }
}
