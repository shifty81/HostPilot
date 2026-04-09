using System.Collections.Concurrent;
using HostPilot.Security.Contracts;

namespace HostPilot.Security.Services;

public sealed class InMemoryNonceStore : INonceStore
{
    private readonly ConcurrentDictionary<string, byte> _nonces = new();

    public Task<bool> IsReplayAsync(string nodeId, string nonce, CancellationToken cancellationToken)
    {
        return Task.FromResult(_nonces.ContainsKey($"{nodeId}:{nonce}"));
    }

    public Task RecordAsync(string nodeId, string nonce, CancellationToken cancellationToken)
    {
        _nonces.TryAdd($"{nodeId}:{nonce}", 0);
        return Task.CompletedTask;
    }
}
