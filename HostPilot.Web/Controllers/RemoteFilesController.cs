namespace HostPilot.Web.Controllers;

using HostPilot.Remote.Files;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/remote-files")]
public sealed class RemoteFilesController : ControllerBase
{
    private readonly RemoteFileBrowserService _service;

    public RemoteFilesController(RemoteFileBrowserService service)
    {
        _service = service;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync(
        [FromQuery] string nodeId,
        [FromQuery] string rootAlias,
        [FromQuery] string rootPath,
        [FromQuery] string requestedPath,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListAsync(nodeId, rootAlias, rootPath, requestedPath, cancellationToken);
        return Ok(result);
    }
}
