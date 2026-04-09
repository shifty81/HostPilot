using HostPilot.Core.Models.Templates;
using HostPilot.Core.Services.Templates;
using Xunit;

namespace HostPilot.Tests.Templates;

public sealed class TemplateLibraryServiceTests
{
    [Fact]
    public async Task Save_And_List_Template_Works()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var service = new FileTemplateLibraryService(dir);

        await service.SaveAsync(new DeploymentTemplateBundle
        {
            DisplayName = "Test Template",
            Slug = "test-template",
            GameId = "rust",
            ProviderId = "steamcmd"
        });

        var all = await service.GetAllAsync();
        Assert.Single(all);
    }
}
