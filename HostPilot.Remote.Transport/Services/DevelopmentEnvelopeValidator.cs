using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Transport.Abstractions;

namespace HostPilot.Remote.Transport.Services;

public sealed class DevelopmentEnvelopeValidator : IEnvelopeValidator
{
    private readonly byte[] _secret;
    private readonly INonceStore _nonceStore;

    public DevelopmentEnvelopeValidator(string secret, INonceStore nonceStore)
    {
        _secret = Encoding.UTF8.GetBytes(secret);
        _nonceStore = nonceStore;
    }

    public async Task<EnvelopeValidationResult> ValidateAsync<T>(SecureEnvelope<T> envelope, CancellationToken cancellationToken)
    {
        if (envelope.ExpiresAtUtc < DateTimeOffset.UtcNow)
        {
            return EnvelopeValidationResult.Failure("Envelope expired.");
        }

        var payloadJson = JsonSerializer.Serialize(envelope.Payload);
        var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payloadJson)));
        if (!string.Equals(expectedHash, envelope.PayloadHash, StringComparison.OrdinalIgnoreCase))
        {
            return EnvelopeValidationResult.Failure("Payload hash mismatch.");
        }

        var registered = await _nonceStore.TryRegisterAsync(envelope.SenderId, envelope.Nonce, envelope.ExpiresAtUtc, cancellationToken);
        if (!registered)
        {
            return EnvelopeValidationResult.Failure("Replay detected.");
        }

        var signatureInput = $"{envelope.MessageId}|{envelope.CorrelationId}|{envelope.SenderId}|{envelope.RecipientId}|{envelope.IssuedAtUtc:O}|{envelope.ExpiresAtUtc:O}|{envelope.Nonce}|{envelope.PayloadHash}";
        using var hmac = new HMACSHA256(_secret);
        var expectedSignature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureInput)));
        if (!string.Equals(expectedSignature, envelope.Signature, StringComparison.OrdinalIgnoreCase))
        {
            return EnvelopeValidationResult.Failure("Signature mismatch.");
        }

        return EnvelopeValidationResult.Success();
    }
}
