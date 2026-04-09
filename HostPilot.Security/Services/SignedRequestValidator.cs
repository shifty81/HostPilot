using HostPilot.Security.Contracts;

namespace HostPilot.Security.Services;

public sealed class SignedRequestValidator
{
    private readonly INodeTrustStore _trustStore;
    private readonly INonceStore _nonceStore;
    private readonly IRequestSigner _requestSigner;

    public SignedRequestValidator(
        INodeTrustStore trustStore,
        INonceStore nonceStore,
        IRequestSigner requestSigner)
    {
        _trustStore = trustStore;
        _nonceStore = nonceStore;
        _requestSigner = requestSigner;
    }

    public async Task ValidateAsync(SignedRequestEnvelope envelope, CancellationToken cancellationToken)
    {
        var trust = await _trustStore.GetAsync(envelope.NodeId, cancellationToken)
            ?? throw new InvalidOperationException("Unknown node.");

        if (trust.IsRevoked || trust.ExpiresUtc < DateTimeOffset.UtcNow)
            throw new InvalidOperationException("Node trust is expired or revoked.");

        if (Math.Abs((DateTimeOffset.UtcNow - envelope.TimestampUtc).TotalMinutes) > 5)
            throw new InvalidOperationException("Signed request timestamp is outside the permitted window.");

        if (await _nonceStore.IsReplayAsync(envelope.NodeId, envelope.Nonce, cancellationToken))
            throw new InvalidOperationException("Signed request replay detected.");

        var message = $"{envelope.NodeId}|{envelope.Nonce}|{envelope.TimestampUtc:O}|{envelope.PayloadJson}";
        if (!_requestSigner.Verify(trust.SharedSecret, message, envelope.Signature))
            throw new InvalidOperationException("Invalid request signature.");

        await _nonceStore.RecordAsync(envelope.NodeId, envelope.Nonce, cancellationToken);
    }
}
