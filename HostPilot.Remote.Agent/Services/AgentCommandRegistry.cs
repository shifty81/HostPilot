using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;

namespace HostPilot.Remote.Agent.Services;

public sealed class AgentCommandRegistry
{
    private readonly Dictionary<string, INodeCommandHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);

    public void Register(INodeCommandHandler handler)
    {
        _handlers[handler.CommandType] = handler;
    }

    public bool TryResolve(string commandType, out INodeCommandHandler? handler)
    {
        return _handlers.TryGetValue(commandType, out handler);
    }

    public IReadOnlyCollection<string> GetKnownCommands() => _handlers.Keys.ToArray();
}
