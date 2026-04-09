using HostPilot.Remote.Agent.Handlers;

namespace HostPilot.Remote.Agent.Services;

public sealed class AgentCommandDispatcher
{
    private readonly IReadOnlyDictionary<string, IAgentCommandHandler> _handlers;

    public AgentCommandDispatcher(IEnumerable<IAgentCommandHandler> handlers)
    {
        _handlers = handlers.ToDictionary(x => x.CommandType, StringComparer.OrdinalIgnoreCase);
    }

    public Task<AgentCommandResult> DispatchAsync(AgentCommandContext context, CancellationToken cancellationToken)
    {
        if (!_handlers.TryGetValue(context.CommandType, out var handler))
        {
            return Task.FromResult(AgentCommandResult.Failure("not_supported", $"No agent handler registered for '{context.CommandType}'."));
        }

        return handler.HandleAsync(context, cancellationToken);
    }
}
