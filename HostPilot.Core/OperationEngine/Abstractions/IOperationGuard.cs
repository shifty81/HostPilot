using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Abstractions;

public interface IOperationGuard
{
    Task<OperationResult?> EvaluateAsync(OperationRequest request, CancellationToken cancellationToken);
}
