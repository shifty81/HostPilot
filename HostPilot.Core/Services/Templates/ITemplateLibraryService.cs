using HostPilot.Core.Models.Templates;

namespace HostPilot.Core.Services.Templates;

public interface ITemplateLibraryService
{
    Task<IReadOnlyList<DeploymentTemplateBundle>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeploymentTemplateBundle> SaveAsync(DeploymentTemplateBundle bundle, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> ExportAsync(Guid id, string targetDirectory, CancellationToken cancellationToken = default);
    Task<DeploymentTemplateBundle> ImportAsync(string bundlePath, CancellationToken cancellationToken = default);
    Task<TemplateReviewResult> ReviewAsync(DeploymentTemplateBundle bundle, CancellationToken cancellationToken = default);
}
