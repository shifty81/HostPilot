using HostPilot.Core.OperationEngine.Abstractions;

namespace HostPilot.Core.OperationEngine.Services;

public sealed class OperationHandlerRegistry
{
    private readonly Dictionary<string, IOperationHandler> _handlers;

    public OperationHandlerRegistry(IEnumerable<IOperationHandler> handlers)
    {
        _handlers = new Dictionary<string, IOperationHandler>(StringComparer.OrdinalIgnoreCase);
        foreach (var handler in handlers)
            Register(handler);
    }

    public void Register(IOperationHandler handler)
    {
        _handlers[handler.OperationType] = handler;
    }

    public bool TryResolve(string operationType, out IOperationHandler? handler) => _handlers.TryGetValue(operationType, out handler);
}
