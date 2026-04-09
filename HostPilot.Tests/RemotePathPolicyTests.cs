using HostPilot.Remote.Files;
using Xunit;

namespace HostPilot.Tests;

public sealed class RemotePathPolicyTests
{
    [Fact]
    public void NormalizeUnderRoot_BlocksTraversal()
    {
        var policy = new RemotePathPolicy();
        Assert.ThrowsAny<Exception>(() => policy.NormalizeUnderRoot(@"/tmp/Servers", @"../../etc/passwd"));
    }

    [Fact]
    public void NormalizeUnderRoot_BlocksSiblingWithSamePrefix()
    {
        var policy = new RemotePathPolicy();
        Assert.ThrowsAny<Exception>(() => policy.NormalizeUnderRoot(@"/tmp/Servers", @"../Servers-Evil/file.txt"));
    }

    [Fact]
    public void NormalizeUnderRoot_AllowsValidSubPath()
    {
        var policy = new RemotePathPolicy();
        var result = policy.NormalizeUnderRoot(@"/tmp/Servers", "game1");
        Assert.StartsWith(Path.GetFullPath(@"/tmp/Servers"), result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NormalizeUnderRoot_AllowsRootItself()
    {
        var policy = new RemotePathPolicy();
        var result = policy.NormalizeUnderRoot(@"/tmp/Servers", string.Empty);
        Assert.Equal(Path.GetFullPath(@"/tmp/Servers"), result);
    }
}
