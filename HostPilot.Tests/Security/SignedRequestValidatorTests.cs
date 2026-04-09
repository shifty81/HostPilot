using HostPilot.Security.Contracts;
using HostPilot.Security.Services;
using Xunit;

namespace HostPilot.Tests.Security;

public sealed class SignedRequestValidatorTests
{
    [Fact]
    public async Task Validate_Succeeds_For_Valid_Signed_Envelope()
    {
        var trustStore = new InMemoryNodeTrustStore();
        var nonceStore = new InMemoryNonceStore();
        var signer = new HmacRequestSigner();
        var validator = new SignedRequestValidator(trustStore, nonceStore, signer);

        var secret = "test-secret";
        await trustStore.SaveAsync(new NodeTrustRecord(
            "node-1", "Node 1", "secret-1", secret,
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(1), false), CancellationToken.None);

        var payload = "{\"kind\":\"heartbeat\"}";
        var timestamp = DateTimeOffset.UtcNow;
        var nonce = "nonce-1";
        var message = $"node-1|{nonce}|{timestamp:O}|{payload}";
        var signature = signer.Sign(secret, message);

        var envelope = new SignedRequestEnvelope("node-1", nonce, timestamp, signature, payload);
        await validator.ValidateAsync(envelope, CancellationToken.None);
    }
}
