using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Abstractions;

public interface IOperationStateStore
{
    void UpsertOperation(OperationRecord record);
    bool TryGetOperation(string operationId, out OperationRecord? record);
    IReadOnlyCollection<OperationRecord> GetAllOperations();
    bool AreDependenciesSatisfied(OperationRequest request);
    void UpsertServerState(ServerStateSnapshot snapshot);
    bool TryGetServerState(string serverId, out ServerStateSnapshot? snapshot);
    IReadOnlyCollection<ServerStateSnapshot> GetAllServerStates();
}
