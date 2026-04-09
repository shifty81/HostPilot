using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Transport.Abstractions;

namespace HostPilot.Remote.Transport.Services;

public sealed class DevelopmentEnvelopeSigner : IEnvelopeSigner
{
    private readonly byte[] _secret;

    public DevelopmentEnvelopeSigner(string secret)
    {
        _secret = Encoding.UTF8.GetBytes(secret);
    }

    public Task<SecureEnvelope<T>> SignAsync<T>(SecureEnvelope<T> envelope, CancellationToken cancellationToken)
    {
        var payloadJson = JsonSerializer.Serialize(envelope.Payload);
        envelope.PayloadHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payloadJson)));

        var signatureInput = $"{envelope.MessageId}|{envelope.CorrelationId}|{envelope.SenderId}|{envelope.RecipientId}|{envelope.IssuedAtUtc:O}|{envelope.ExpiresAtUtc:O}|{envelope.Nonce}|{envelope.PayloadHash}";
        using var hmac = new HMACSHA256(_secret);
        envelope.Signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureInput)));
        return Task.FromResult(envelope);
    }
}
