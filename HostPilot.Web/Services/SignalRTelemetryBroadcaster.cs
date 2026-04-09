using HostPilot.Telemetry.Contracts;
using HostPilot.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HostPilot.Web.Services;

/// <summary>
/// Implements <see cref="ITelemetryBroadcaster"/> by pushing events to all connected
/// <see cref="ClusterTelemetryHub"/> clients.
/// </summary>
public sealed class SignalRTelemetryBroadcaster : ITelemetryBroadcaster
{
    private readonly IHubContext<ClusterTelemetryHub> _hubContext;

    public SignalRTelemetryBroadcaster(IHubContext<ClusterTelemetryHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BroadcastAsync(string topic, object payload, CancellationToken cancellationToken)
        => _hubContext.Clients.All.SendAsync(topic, payload, cancellationToken);
}
