namespace HostPilot.Core.OperationEngine.Services;

/// <summary>
/// Provides per-server exclusive locks so that only one destructive operation
/// (install, start, stop, restart, backup) can run on a given server at a time.
/// </summary>
public sealed class ServerLockService
{
    private readonly HashSet<string> _lockedServerIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    /// <summary>Returns <c>true</c> and acquires the lock if the server is not already locked.</summary>
    public bool TryAcquire(string serverId)
    {
        lock (_sync)
        {
            return _lockedServerIds.Add(serverId);
        }
    }

    /// <summary>Releases the lock held for the given server.</summary>
    public void Release(string serverId)
    {
        lock (_sync)
        {
            _lockedServerIds.Remove(serverId);
        }
    }

    /// <summary>Returns <c>true</c> if the server currently holds a lock.</summary>
    public bool IsLocked(string serverId)
    {
        lock (_sync)
        {
            return _lockedServerIds.Contains(serverId);
        }
    }
}
