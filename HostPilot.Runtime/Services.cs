using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Contracts;

namespace HostPilot.Runtime;

public sealed class RemoteNodeRegistry : IRemoteNodeRegistry
{
    private readonly ConcurrentDictionary<string, RemoteNodeDto> _nodes = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<RemoteNodeDto> GetAll() => _nodes.Values.OrderBy(x => x.DisplayName).ToArray();

    public RemoteNodeDto? GetById(string nodeId)
        => _nodes.TryGetValue(nodeId, out var node) ? node : null;

    public void Upsert(RemoteNodeDto node)
    {
        ArgumentNullException.ThrowIfNull(node);
        _nodes[node.NodeId] = node;
    }

    public bool Remove(string nodeId) => _nodes.TryRemove(nodeId, out _);
}

public sealed class RemoteNodeLeaseService : IRemoteNodeLeaseService
{
    private readonly ConcurrentDictionary<string, NodeHeartbeatDto> _heartbeats = new(StringComparer.OrdinalIgnoreCase);

    public void RecordHeartbeat(NodeHeartbeatDto heartbeat)
    {
        ArgumentNullException.ThrowIfNull(heartbeat);
        _heartbeats[heartbeat.NodeId] = heartbeat;
    }

    public bool IsFresh(string nodeId, TimeSpan maxAge)
    {
        if (!_heartbeats.TryGetValue(nodeId, out var heartbeat))
        {
            return false;
        }

        return DateTimeOffset.UtcNow - heartbeat.TimestampUtc <= maxAge;
    }
}

public sealed class AuthorizationPolicyService : IAuthorizationPolicyService
{
    public bool HasPermission(PermissionGrantDto grant, string permission)
    {
        ArgumentNullException.ThrowIfNull(grant);
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);

        return grant.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase)
            || grant.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class RemoteCommandRouter : IRemoteCommandRouter
{
    private readonly IRemoteNodeTransport _transport;
    private readonly IWebAuditService _auditService;

    public RemoteCommandRouter(IRemoteNodeTransport transport, IWebAuditService auditService)
    {
        _transport = transport;
        _auditService = auditService;
    }

    public async Task<OperationStatusDto> DispatchAsync(NodeCommandEnvelope command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _auditService.WriteAsync(
            command.RequestedBy,
            command.CommandType.ToString(),
            $"node:{command.NodeId}/server:{command.TargetServerId}",
            "Remote command dispatched.",
            cancellationToken);

        return await _transport.SendCommandAsync(command, cancellationToken);
    }
}

public sealed class InMemoryAuditService : IWebAuditService
{
    public Task WriteAsync(string actor, string action, string target, string details, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
