namespace HostPilot.Web.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize(Policy = "RemoteControl")]
public sealed class RuntimeEventsHub : Hub
{
    public Task SubscribeNode(string nodeId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"node:{nodeId}");

    public Task UnsubscribeNode(string nodeId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"node:{nodeId}");
}
