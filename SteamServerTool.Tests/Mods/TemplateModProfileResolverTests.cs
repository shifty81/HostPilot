using SteamServerTool.Core.Models;
using SteamServerTool.Core.Services.Mods;
using Xunit;

namespace SteamServerTool.Tests.Mods;

public sealed class TemplateModProfileResolverTests
{
    [Fact]
    public void Resolve_ReturnsMinecraftBrowserProfile()
    {
        var resolver = new TemplateModProfileResolver();
        var template = new ServerTemplate { Name = "Minecraft Forge" };

        var profile = resolver.Resolve(template);

        Assert.True(profile.SupportsBrowser);
        Assert.Contains(".jar", profile.AcceptedLocalExtensions);
        Assert.Equal("mods", profile.InstallRootRelativePath);
    }

    [Fact]
    public void Resolve_ReturnsVintageStoryProfile()
    {
        var resolver = new TemplateModProfileResolver();
        var template = new ServerTemplate { Name = "Vintage Story" };

        var profile = resolver.Resolve(template);

        Assert.True(profile.AcceptFolders);
        Assert.Equal("Mods", profile.InstallRootRelativePath);
    }
}
