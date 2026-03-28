using HostPilot.Core.Services.Mods;

namespace HostPilot.Tests.Mods;

public sealed class SteamWorkshopUrlParserTests
{
    [Theory]
    [InlineData("3074561085", "3074561085")]
    [InlineData("https://steamcommunity.com/sharedfiles/filedetails/?id=3074561085", "3074561085")]
    [InlineData("https://steamcommunity.com/workshop/filedetails/?id=3074561085&searchtext=mod", "3074561085")]
    public void ExtractPublishedFileId_ReturnsId_WhenInputContainsWorkshopId(string input, string expected)
    {
        var actual = SteamWorkshopUrlParser.ExtractPublishedFileId(input);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ExtractPublishedFileId_ReturnsNull_WhenInputDoesNotContainWorkshopId()
    {
        Assert.Null(SteamWorkshopUrlParser.ExtractPublishedFileId("some random text"));
    }
}
