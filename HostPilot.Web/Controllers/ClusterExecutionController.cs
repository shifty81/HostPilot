using HostPilot.Core.Execution.Contracts;
using HostPilot.Core.Execution.Services;
using Microsoft.AspNetCore.Mvc;

namespace HostPilot.Web.Controllers;

/// <summary>
/// Accepts distributed execution plan submissions and hands them to the
/// <see cref="DistributedExecutionScheduler"/> for multi-node dispatch.
/// </summary>
[ApiController]
[Route("api/execution")]
public sealed class ClusterExecutionController : ControllerBase
{
    private readonly DistributedExecutionScheduler _scheduler;

    public ClusterExecutionController(DistributedExecutionScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    /// <summary>Schedules an execution plan across available nodes.</summary>
    [HttpPost("plans/run")]
    public async Task<IActionResult> RunPlan([FromBody] ExecutionPlan plan, CancellationToken cancellationToken)
    {
        await _scheduler.ScheduleAsync(plan, cancellationToken);
        return Accepted(new { plan.PlanId, Status = "Scheduled" });
    }
}
