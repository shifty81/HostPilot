namespace HostPilot.Web.Pages;

using HostPilot.Remote.Contracts.Models;
using HostPilot.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

public sealed class NodesModel : PageModel
{
    private readonly NodeDashboardStore _store;

    public NodesModel(NodeDashboardStore store)
    {
        _store = store;
    }

    public IReadOnlyList<RemoteNodeIdentity> Nodes { get; private set; } = Array.Empty<RemoteNodeIdentity>();

    public void OnGet()
    {
        Nodes = _store.GetNodes();
    }
}
