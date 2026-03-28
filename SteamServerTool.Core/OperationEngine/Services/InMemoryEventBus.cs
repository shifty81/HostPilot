using SteamServerTool.Core.OperationEngine.Abstractions;

namespace SteamServerTool.Core.OperationEngine.Services;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly object _sync = new();
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Publish<TEvent>(TEvent @event)
    {
        List<Delegate> handlers;
        lock (_sync)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var registered))
                return;
            handlers = registered.ToList();
        }

        foreach (var handler in handlers.Cast<Action<TEvent>>())
            handler(@event);
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
    {
        lock (_sync)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<Delegate>();
                _handlers[typeof(TEvent)] = handlers;
            }

            handlers.Add(handler);
        }

        return new Subscription(() =>
        {
            lock (_sync)
            {
                if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
                    handlers.Remove(handler);
            }
        });
    }

    private sealed class Subscription(Action dispose) : IDisposable
    {
        private int _disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                dispose();
        }
    }
}
