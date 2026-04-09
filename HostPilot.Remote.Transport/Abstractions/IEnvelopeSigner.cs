using HostPilot.Remote.Contracts.Models;

namespace HostPilot.Remote.Transport.Abstractions;

public interface IEnvelopeSigner
{
    Task<SecureEnvelope<T>> SignAsync<T>(SecureEnvelope<T> envelope, CancellationToken cancellationToken);
}
