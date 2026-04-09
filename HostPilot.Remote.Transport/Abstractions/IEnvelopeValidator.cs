using HostPilot.Remote.Contracts.Models;

namespace HostPilot.Remote.Transport.Abstractions;

public interface IEnvelopeValidator
{
    Task<EnvelopeValidationResult> ValidateAsync<T>(SecureEnvelope<T> envelope, CancellationToken cancellationToken);
}
