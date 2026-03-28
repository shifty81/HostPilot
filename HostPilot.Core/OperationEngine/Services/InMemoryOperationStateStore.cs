using System.Collections.Concurrent;
using HostPilot.Core.OperationEngine.Abstractions;
using HostPilot.Core.OperationEngine.Models;

namespace HostPilot.Core.OperationEngine.Services;

public sealed class InMemoryOperationStateStore : IOperationStateStore
{
    private readonly ConcurrentDictionary<string, OperationRecord> _operations = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, ServerStateSnapshot> _serverStates = new(StringComparer.OrdinalIgnoreCase);

    public void UpsertOperation(OperationRecord record) => _operations[record.OperationId] = record;

    public bool TryGetOperation(string operationId, out OperationRecord? record) => _operations.TryGetValue(operationId, out record);

    public IReadOnlyCollection<OperationRecord> GetAllOperations() => _operations.Values.OrderByDescending(x => x.CreatedAtUtc).ToArray();

    public bool AreDependenciesSatisfied(OperationRequest request)
    {
        if (request.DependsOnOperationIds.Count == 0)
            return true;

        foreach (var dependency in request.DependsOnOperationIds)
        {
            if (!_operations.TryGetValue(dependency, out var dependencyRecord))
                return false;

            if (dependencyRecord.Status != OperationStatus.Succeeded)
                return false;
        }

        return true;
    }

    public void UpsertServerState(ServerStateSnapshot snapshot) => _serverStates[snapshot.ServerId] = snapshot;

    public bool TryGetServerState(string serverId, out ServerStateSnapshot? snapshot) => _serverStates.TryGetValue(serverId, out snapshot);

    public IReadOnlyCollection<ServerStateSnapshot> GetAllServerStates() => _serverStates.Values.OrderBy(x => x.ServerId).ToArray();
}
