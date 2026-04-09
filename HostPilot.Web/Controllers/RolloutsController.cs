namespace HostPilot.Web.Controllers;

using HostPilot.Web.Models;
using HostPilot.Web.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/rollouts")]
public sealed class RolloutsController : ControllerBase
{
    private readonly RolloutCoordinator _coordinator;

    public RolloutsController(RolloutCoordinator coordinator)
    {
        _coordinator = coordinator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] RolloutPlanDto request,
        CancellationToken cancellationToken)
    {
        await _coordinator.SavePlanAsync(request, cancellationToken);
        return Ok(request);
    }

    [HttpGet("{rolloutId}/states")]
    public async Task<IActionResult> GetStatesAsync(
        [FromRoute] string rolloutId,
        CancellationToken cancellationToken)
    {
        var result = await _coordinator.GetStatesAsync(rolloutId, cancellationToken);
        return Ok(result);
    }
}
