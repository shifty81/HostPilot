using HostPilot.Contracts;
using HostPilot.Runtime;
using Xunit;

namespace HostPilot.Tests;

public sealed class RemoteNodeRegistryTests
{
    [Fact]
    public void Upsert_ShouldStoreNode()
    {
        var registry = new RemoteNodeRegistry();
        registry.Upsert(new RemoteNodeDto { NodeId = "node-01", DisplayName = "Node 01" });

        var result = registry.GetById("node-01");
        Assert.NotNull(result);
        Assert.Equal("Node 01", result!.DisplayName);
    }
}
