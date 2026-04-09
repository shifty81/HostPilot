using HostPilot.Core.Models.Nodes;

namespace HostPilot.Core.Services.Nodes;

public interface INodeEnrollmentService
{
    Task<NodeEnrollmentToken> CreateEnrollmentTokenAsync(Guid nodeId, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task<bool> ValidateAndConsumeTokenAsync(Guid nodeId, string token, CancellationToken cancellationToken = default);
}
