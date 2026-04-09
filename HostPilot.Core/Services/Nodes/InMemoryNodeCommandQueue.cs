using System.Collections.Concurrent;
using HostPilot.Core.Models.Nodes;

namespace HostPilot.Core.Services.Nodes;

public sealed class InMemoryNodeCommandQueue : INodeCommandQueue
{
    private readonly ConcurrentDictionary<Guid, NodeCommandEnvelope> _commands = new();

    public Task<NodeCommandEnvelope> EnqueueAsync(NodeCommandEnvelope command, CancellationToken cancellationToken = default)
    {
        _commands[command.CommandId] = command;
        return Task.FromResult(command);
    }

    public Task<IReadOnlyList<NodeCommandEnvelope>> DequeuePendingAsync(Guid nodeId, int maxCount, CancellationToken cancellationToken = default)
    {
        var list = _commands.Values
            .Where(x => x.NodeId == nodeId && x.Status == "Pending")
            .OrderBy(x => x.CreatedUtc)
            .Take(maxCount)
            .ToList();

        foreach (var item in list)
        {
            item.Status = "Claimed";
            item.ClaimedUtc = DateTimeOffset.UtcNow;
        }

        return Task.FromResult<IReadOnlyList<NodeCommandEnvelope>>(list);
    }

    public Task CompleteAsync(Guid commandId, string resultJson, CancellationToken cancellationToken = default)
    {
        if (_commands.TryGetValue(commandId, out var cmd))
        {
            cmd.Status = "Completed";
            cmd.CompletedUtc = DateTimeOffset.UtcNow;
            cmd.ResultJson = resultJson;
        }

        return Task.CompletedTask;
    }

    public Task FailAsync(Guid commandId, string error, CancellationToken cancellationToken = default)
    {
        if (_commands.TryGetValue(commandId, out var cmd))
        {
            cmd.Status = "Failed";
            cmd.CompletedUtc = DateTimeOffset.UtcNow;
            cmd.Error = error;
        }

        return Task.CompletedTask;
    }
}
