using System.Collections.ObjectModel;

namespace HostPilot.Core.OperationEngine.Models;

public sealed class OperationContext
{
    public OperationContext(string operationId, string operationType, string targetId, IReadOnlyDictionary<string, string>? metadata = null)
    {
        OperationId = operationId;
        OperationType = operationType;
        TargetId = targetId;
        Metadata = metadata ?? new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
    }

    public string OperationId { get; }
    public string OperationType { get; }
    public string TargetId { get; }
    public IReadOnlyDictionary<string, string> Metadata { get; }
}
