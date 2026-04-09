using HostPilot.Core.Services.Nodes;
using Xunit;

namespace HostPilot.Tests.Nodes;

public sealed class NodeEnrollmentServiceTests
{
    [Fact]
    public async Task Token_Can_Be_Created_Validated_And_Consumed()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");
        var service = new FileNodeEnrollmentService(path);

        var nodeId = Guid.NewGuid();
        var token = await service.CreateEnrollmentTokenAsync(nodeId, TimeSpan.FromMinutes(5));

        var first = await service.ValidateAndConsumeTokenAsync(nodeId, token.Token);
        var second = await service.ValidateAndConsumeTokenAsync(nodeId, token.Token);

        Assert.True(first);
        Assert.False(second);
    }
}
