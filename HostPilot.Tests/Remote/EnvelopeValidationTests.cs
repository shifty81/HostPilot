using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Transport.Services;
using Xunit;

namespace HostPilot.Tests;

public sealed class EnvelopeValidationTests
{
    [Fact]
    public async Task SignedEnvelope_ShouldValidate()
    {
        var nonceStore = new InMemoryNonceStore();
        var signer = new DevelopmentEnvelopeSigner("test-secret");
        var validator = new DevelopmentEnvelopeValidator("test-secret", nonceStore);

        var envelope = new SecureEnvelope<RemoteCommandRequest>
        {
            MessageId = Guid.NewGuid().ToString("N"),
            CorrelationId = Guid.NewGuid().ToString("N"),
            SenderId = "controller",
            RecipientId = "node-a",
            IssuedAtUtc = DateTimeOffset.UtcNow,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            Nonce = Guid.NewGuid().ToString("N"),
            Payload = new RemoteCommandRequest
            {
                CommandId = Guid.NewGuid().ToString("N"),
                CorrelationId = Guid.NewGuid().ToString("N"),
                NodeId = "node-a",
                ServerId = "server-a",
                CommandType = "restart",
                RequestedBy = "tester",
                RequestedAtUtc = DateTimeOffset.UtcNow,
                Arguments = new Dictionary<string, string>()
            }
        };

        envelope = await signer.SignAsync(envelope, CancellationToken.None);
        var result = await validator.ValidateAsync(envelope, CancellationToken.None);

        Assert.True(result.IsValid, result.FailureReason);
    }
}
