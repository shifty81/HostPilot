using SteamServerTool.Core.OperationEngine.Models;

namespace SteamServerTool.Core.OperationEngine.Abstractions;

public interface IOperationGuard
{
    Task<OperationResult?> EvaluateAsync(OperationRequest request, CancellationToken cancellationToken);
}
