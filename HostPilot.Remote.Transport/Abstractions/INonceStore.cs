namespace HostPilot.Remote.Transport.Abstractions;

public interface INonceStore
{
    Task<bool> TryRegisterAsync(string senderId, string nonce, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken);
}
