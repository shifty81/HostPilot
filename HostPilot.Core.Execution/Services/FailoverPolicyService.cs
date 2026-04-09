using HostPilot.Core.Execution.Contracts;

namespace HostPilot.Core.Execution.Services;

public sealed class FailoverPolicyService
{
    public bool ShouldAttemptFailover(WorkItemRuntimeState state, int maxAttempts)
    {
        return state.State == WorkItemState.Failed && state.Attempt < maxAttempts;
    }

    public string BuildFailoverReason(string originalNodeId)
    {
        return $"Failover triggered because node '{originalNodeId}' did not complete the workload successfully.";
    }
}
