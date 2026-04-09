namespace HostPilot.Web.Controllers;

using HostPilot.Contracts;
using HostPilot.Runtime;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/nodes")]
public sealed class NodesController : ControllerBase
{
    private readonly IRemoteNodeRegistry _registry;
    private readonly IRemoteNodeLeaseService _leaseService;

    public NodesController(IRemoteNodeRegistry registry, IRemoteNodeLeaseService leaseService)
    {
        _registry = registry;
        _leaseService = leaseService;
    }

    [HttpGet]
    public ActionResult GetAll() => Ok(_registry.GetAll());

    [HttpPost("heartbeat")]
    public IActionResult Heartbeat([FromBody] NodeHeartbeatDto heartbeat)
    {
        _leaseService.RecordHeartbeat(heartbeat);
        return Accepted();
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RemoteNodeDto node)
    {
        _registry.Upsert(node);
        return Accepted(node);
    }
}
