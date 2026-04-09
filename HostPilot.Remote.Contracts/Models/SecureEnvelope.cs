namespace HostPilot.Remote.Contracts.Models;

public sealed class SecureEnvelope<T>
{
    public string MessageId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string RecipientId { get; set; } = string.Empty;
    public DateTimeOffset IssuedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public string Nonce { get; set; } = string.Empty;
    public string PayloadHash { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public IReadOnlyList<string> Scopes { get; set; } = Array.Empty<string>();
    public T? Payload { get; set; }
}
