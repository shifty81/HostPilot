namespace HostPilot.Web.Services;

using HostPilot.Contracts;
using HostPilot.Runtime;

public sealed class StubRemoteNodeTransport : IRemoteNodeTransport
{
    public Task<OperationStatusDto> SendCommandAsync(NodeCommandEnvelope command, CancellationToken cancellationToken)
    {
        return Task.FromResult(new OperationStatusDto
        {
            OperationId = Guid.NewGuid(),
            CorrelationId = command.CorrelationId,
            State = OperationExecutionState.Accepted,
            Summary = $"Accepted {command.CommandType} for node '{command.NodeId}'.",
            NodeId = command.NodeId,
            TargetServerId = command.TargetServerId,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        });
    }
}
