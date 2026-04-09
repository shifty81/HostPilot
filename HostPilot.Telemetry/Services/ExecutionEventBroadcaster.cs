using HostPilot.Core.Execution.Contracts;
using HostPilot.Telemetry.Contracts;

namespace HostPilot.Telemetry.Services;

public sealed class ExecutionEventBroadcaster : IExecutionEventSink
{
    private readonly ITelemetryBroadcaster _broadcaster;

    public ExecutionEventBroadcaster(ITelemetryBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    public Task PublishAsync(ExecutionEvent evt, CancellationToken cancellationToken)
    {
        return _broadcaster.BroadcastAsync("execution-events", evt, cancellationToken);
    }
}
