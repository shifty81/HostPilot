using System.Collections.Concurrent;
using HostPilot.Remote.Transport.Abstractions;

namespace HostPilot.Remote.Transport.Services;

public sealed class InMemoryNonceStore : INonceStore
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _entries = new();

    public Task<bool> TryRegisterAsync(string senderId, string nonce, DateTimeOffset expiresAtUtc, CancellationToken cancellationToken)
    {
        var key = $"{senderId}:{nonce}";
        CleanupExpired();
        var added = _entries.TryAdd(key, expiresAtUtc);
        return Task.FromResult(added);
    }

    private void CleanupExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var pair in _entries)
        {
            if (pair.Value <= now)
            {
                _entries.TryRemove(pair.Key, out _);
            }
        }
    }
}
