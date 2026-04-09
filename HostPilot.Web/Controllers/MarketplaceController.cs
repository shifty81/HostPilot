using HostPilot.Core.Services.Templates;
using Microsoft.AspNetCore.Mvc;

namespace HostPilot.Web.Controllers;

/// <summary>Public-facing marketplace endpoints for discovering featured templates.</summary>
[ApiController]
[Route("api/marketplace")]
public sealed class MarketplaceController : ControllerBase
{
    private readonly ITemplateLibraryService _templateLibraryService;

    public MarketplaceController(ITemplateLibraryService templateLibraryService)
    {
        _templateLibraryService = templateLibraryService;
    }

    /// <summary>Returns the first 12 templates as featured marketplace items.</summary>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured(CancellationToken cancellationToken)
    {
        var all = await _templateLibraryService.GetAllAsync(cancellationToken);
        var featured = all.Take(12).Select(x => new
        {
            x.Id,
            x.DisplayName,
            x.Description,
            x.GameId,
            x.ProviderId,
            x.Version,
            x.Author,
            x.Tags
        });

        return Ok(featured);
    }
}
