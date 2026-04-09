using HostPilot.Core.Models.Nodes;
using HostPilot.Core.Services.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace HostPilot.Web.Controllers;

/// <summary>
/// Handles agent node enrollment, heartbeats, and command polling.
/// All routes under this controller are validated by
/// <see cref="Middleware.SignedRequestValidationMiddleware"/> at runtime.
/// </summary>
[ApiController]
[Route("api/agent")]
public sealed class AgentController : ControllerBase
{
    private readonly INodeRegistryService _nodeRegistryService;
    private readonly INodeEnrollmentService _nodeEnrollmentService;
    private readonly INodeCommandQueue _nodeCommandQueue;

    public AgentController(
        INodeRegistryService nodeRegistryService,
        INodeEnrollmentService nodeEnrollmentService,
        INodeCommandQueue nodeCommandQueue)
    {
        _nodeRegistryService = nodeRegistryService;
        _nodeEnrollmentService = nodeEnrollmentService;
        _nodeCommandQueue = nodeCommandQueue;
    }

    /// <summary>Validates an enrollment token and registers the calling node.</summary>
    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request, CancellationToken cancellationToken)
    {
        var ok = await _nodeEnrollmentService.ValidateAndConsumeTokenAsync(request.NodeId, request.Token, cancellationToken);
        if (!ok)
            return Unauthorized();

        var existing = await _nodeRegistryService.GetAsync(request.NodeId, cancellationToken)
            ?? new ManagedNode
            {
                Id = request.NodeId,
                DisplayName = $"Node-{request.NodeId.ToString()[..8]}",
                Hostname = request.NodeId.ToString()
            };

        await _nodeRegistryService.UpsertAsync(existing, cancellationToken);
        return Ok(new { ok = true });
    }

    /// <summary>Records a live heartbeat from an enrolled node.</summary>
    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat([FromBody] NodeHeartbeat heartbeat, CancellationToken cancellationToken)
    {
        await _nodeRegistryService.UpdateHeartbeatAsync(heartbeat, cancellationToken);
        return Ok(new { ok = true });
    }

    /// <summary>Returns up to <paramref name="max"/> pending commands for the node.</summary>
    [HttpGet("commands/{nodeId:guid}")]
    public Task<IReadOnlyList<NodeCommandEnvelope>> GetCommands(Guid nodeId, int max = 20, CancellationToken cancellationToken = default)
        => _nodeCommandQueue.DequeuePendingAsync(nodeId, max, cancellationToken);

    /// <summary>Marks a dispatched command as completed.</summary>
    [HttpPost("commands/{commandId:guid}/complete")]
    public async Task<IActionResult> Complete(Guid commandId, [FromBody] object result, CancellationToken cancellationToken)
    {
        await _nodeCommandQueue.CompleteAsync(commandId, System.Text.Json.JsonSerializer.Serialize(result), cancellationToken);
        return Ok(new { ok = true });
    }

    /// <summary>Marks a dispatched command as failed.</summary>
    [HttpPost("commands/{commandId:guid}/fail")]
    public async Task<IActionResult> Fail(Guid commandId, [FromBody] FailRequest request, CancellationToken cancellationToken)
    {
        await _nodeCommandQueue.FailAsync(commandId, request.Error, cancellationToken);
        return Ok(new { ok = true });
    }

    public sealed class EnrollRequest
    {
        public Guid NodeId { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public sealed class FailRequest
    {
        public string Error { get; set; } = string.Empty;
    }
}
