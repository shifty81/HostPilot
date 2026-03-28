using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Abstractions;

public interface IOperationEngine
{
    Task<string> QueueAsync(OperationRequest request, CancellationToken cancellationToken = default);
    bool TryGetOperation(string operationId, out OperationRecord? record);
    IReadOnlyCollection<OperationRecord> GetOperations();
}
