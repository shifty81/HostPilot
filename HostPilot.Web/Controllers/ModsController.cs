using HostPilot.Core.Models.Mods;
using HostPilot.Core.Services.Mods;
using Microsoft.AspNetCore.Mvc;

namespace HostPilot.Web.Controllers;

/// <summary>Exposes mod catalog search and local import endpoints.</summary>
[ApiController]
[Route("api/mods")]
public sealed class ModsController : ControllerBase
{
    private readonly IModCatalogService _modCatalogService;

    public ModsController(IModCatalogService modCatalogService)
    {
        _modCatalogService = modCatalogService;
    }

    /// <summary>Searches the mod catalog across all registered providers.</summary>
    [HttpPost("search")]
    public Task<IReadOnlyList<ModPackage>> Search([FromBody] ModSearchQuery query, CancellationToken cancellationToken)
        => _modCatalogService.SearchAsync(query, cancellationToken);

    /// <summary>Imports locally-present mod files into the catalog.</summary>
    [HttpPost("import-local")]
    public Task<IReadOnlyList<ModPackage>> ImportLocal([FromBody] LocalModImportRequest request, CancellationToken cancellationToken)
        => _modCatalogService.ImportLocalAsync(request, cancellationToken);
}
