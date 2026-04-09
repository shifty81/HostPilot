using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HostPilot.Contracts;

namespace HostPilot.Runtime;

public interface IRemoteNodeRegistry
{
    IReadOnlyCollection<RemoteNodeDto> GetAll();
    RemoteNodeDto? GetById(string nodeId);
    void Upsert(RemoteNodeDto node);
    bool Remove(string nodeId);
}

public interface IRemoteNodeLeaseService
{
    void RecordHeartbeat(NodeHeartbeatDto heartbeat);
    bool IsFresh(string nodeId, TimeSpan maxAge);
}

public interface IRemoteNodeTransport
{
    Task<OperationStatusDto> SendCommandAsync(NodeCommandEnvelope command, CancellationToken cancellationToken);
}

public interface IRemoteCommandRouter
{
    Task<OperationStatusDto> DispatchAsync(NodeCommandEnvelope command, CancellationToken cancellationToken);
}

public interface IWebAuditService
{
    Task WriteAsync(string actor, string action, string target, string details, CancellationToken cancellationToken);
}

public interface IAuthorizationPolicyService
{
    bool HasPermission(PermissionGrantDto grant, string permission);
}

public interface IRuntimeEventBroadcaster
{
    Task PublishNodeHeartbeatAsync(NodeHeartbeatDto heartbeat, CancellationToken cancellationToken);
    Task PublishOperationUpdateAsync(OperationStatusDto update, CancellationToken cancellationToken);
}
