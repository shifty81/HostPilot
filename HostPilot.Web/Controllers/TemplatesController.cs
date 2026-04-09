using HostPilot.Core.Models.Templates;
using HostPilot.Core.Services.Templates;
using Microsoft.AspNetCore.Mvc;

namespace HostPilot.Web.Controllers;

/// <summary>CRUD and export operations for deployment template bundles.</summary>
[ApiController]
[Route("api/templates")]
public sealed class TemplatesController : ControllerBase
{
    private readonly ITemplateLibraryService _templateLibraryService;

    public TemplatesController(ITemplateLibraryService templateLibraryService)
    {
        _templateLibraryService = templateLibraryService;
    }

    /// <summary>Returns all available deployment templates.</summary>
    [HttpGet]
    public Task<IReadOnlyList<DeploymentTemplateBundle>> GetAll(CancellationToken cancellationToken)
        => _templateLibraryService.GetAllAsync(cancellationToken);

    /// <summary>Creates or updates a template bundle.</summary>
    [HttpPost]
    public Task<DeploymentTemplateBundle> Save([FromBody] DeploymentTemplateBundle bundle, CancellationToken cancellationToken)
        => _templateLibraryService.SaveAsync(bundle, cancellationToken);

    /// <summary>Reviews a template bundle for correctness before saving.</summary>
    [HttpPost("review")]
    public Task<TemplateReviewResult> Review([FromBody] DeploymentTemplateBundle bundle, CancellationToken cancellationToken)
        => _templateLibraryService.ReviewAsync(bundle, cancellationToken);

    /// <summary>Exports a template bundle to a JSON file under the exports directory.</summary>
    [HttpPost("{id:guid}/export")]
    public Task<string> Export(Guid id, CancellationToken cancellationToken)
    {
        var exportDir = Path.Combine(AppContext.BaseDirectory, "Exports");
        return _templateLibraryService.ExportAsync(id, exportDir, cancellationToken);
    }
}
