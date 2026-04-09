namespace HostPilot.Web.Controllers;

using HostPilot.Contracts;
using HostPilot.Runtime;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IRemoteNodeRegistry _registry;

    public DashboardController(IRemoteNodeRegistry registry)
    {
        _registry = registry;
    }

    [HttpGet("summary")]
    public ActionResult<DashboardSummaryDto> GetSummary()
    {
        var nodes = _registry.GetAll();
        return Ok(new DashboardSummaryDto
        {
            TotalNodes = nodes.Count,
            OnlineNodes = nodes.Count(x => x.Status == RemoteNodeStatus.Online),
            OfflineNodes = nodes.Count(x => x.Status == RemoteNodeStatus.Offline),
            TotalServers = 0,
            RunningServers = 0,
            ActiveOperations = 0,
            ActiveAlerts = 0
        });
    }
}
