namespace HostPilot.Remote.Updates.Services;

using HostPilot.Remote.Contracts.Models;

public sealed class InMemoryUpdateStateStore
{
    private readonly object _lock = new();
    private readonly List<RemoteUpdateProgress> _items = new();

    public IReadOnlyList<RemoteUpdateProgress> GetAll()
    {
        lock (_lock)
        {
            return _items.ToArray();
        }
    }

    public void Add(RemoteUpdateProgress progress)
    {
        lock (_lock)
        {
            _items.Add(progress);
        }
    }
}
