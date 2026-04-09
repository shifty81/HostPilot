namespace HostPilot.Remote.Updates.Services;

using HostPilot.Remote.Contracts.Models;

public sealed class InMemoryUpdateStateStore
{
    private readonly List<RemoteUpdateProgress> _items = new();

    public IReadOnlyList<RemoteUpdateProgress> GetAll() => _items;
    public void Add(RemoteUpdateProgress progress) => _items.Add(progress);
}
