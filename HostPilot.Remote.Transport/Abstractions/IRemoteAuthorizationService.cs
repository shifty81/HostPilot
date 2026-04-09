namespace HostPilot.Remote.Transport.Abstractions;

public interface IRemoteAuthorizationService
{
    Task<bool> IsAllowedAsync(string principalId, string action, string targetNodeId, string? targetServerId, CancellationToken cancellationToken);
}
