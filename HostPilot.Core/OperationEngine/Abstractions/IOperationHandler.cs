using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Abstractions;

public interface IOperationHandler
{
    string OperationType { get; }
    Task<OperationResult> ExecuteAsync(OperationContext context, IReadOnlyDictionary<string, object?> payload, CancellationToken cancellationToken);
}
