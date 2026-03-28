namespace HostPilot.Core.OperationEngine.Abstractions;

public interface IEventBus
{
    void Publish<TEvent>(TEvent @event);
    IDisposable Subscribe<TEvent>(Action<TEvent> handler);
}
