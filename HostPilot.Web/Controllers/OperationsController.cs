namespace HostPilot.Web.Controllers;

using HostPilot.Contracts;
using HostPilot.Runtime;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/operations")]
public sealed class OperationsController : ControllerBase
{
    private readonly IRemoteCommandRouter _router;

    public OperationsController(IRemoteCommandRouter router)
    {
        _router = router;
    }

    [HttpPost("dispatch")]
    public async Task<ActionResult<OperationStatusDto>> DispatchAsync(
        [FromBody] NodeCommandEnvelope command,
        CancellationToken cancellationToken)
    {
        var result = await _router.DispatchAsync(command, cancellationToken);
        return Accepted(result);
    }
}
