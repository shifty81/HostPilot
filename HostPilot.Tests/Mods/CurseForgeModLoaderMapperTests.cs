using Xunit;
using HostPilot.Core.Services.Mods;

namespace HostPilot.Tests.Mods;

public sealed class CurseForgeModLoaderMapperTests
{
    [Theory]
    [InlineData("forge", 1)]
    [InlineData("fabric", 4)]
    [InlineData("neoforge", 6)]
    public void TryMap_ReturnsKnownLoaderIds(string loader, int expected)
    {
        Assert.Equal(expected, CurseForgeModLoaderMapper.TryMap(loader));
    }

    [Fact]
    public void TryMap_ReturnsNull_ForUnknownLoader()
    {
        Assert.Null(CurseForgeModLoaderMapper.TryMap("paper"));
    }
}
