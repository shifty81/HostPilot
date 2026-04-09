using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Execution.Services;
using Xunit;

namespace HostPilot.Tests.Remote;

public sealed class CommandPolicyTests
{
    [Fact]
    public async Task Throws_When_Node_Is_Missing()
    {
        var policy = new DefaultCommandExecutionPolicy();
        var descriptor = new RemoteCommandDescriptor { CommandKey = "server.start", DisplayName = "Start" };
        var request = new RemoteExecutionRequest { RequestedBy = "tester" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => policy.EnsureAllowedAsync(request, descriptor));
    }

    [Fact]
    public async Task Throws_When_RequestedBy_Is_Missing()
    {
        var policy = new DefaultCommandExecutionPolicy();
        var descriptor = new RemoteCommandDescriptor { CommandKey = "server.start", DisplayName = "Start" };
        var request = new RemoteExecutionRequest { NodeId = "node-1" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => policy.EnsureAllowedAsync(request, descriptor));
    }

    [Fact]
    public async Task Succeeds_With_Valid_Request()
    {
        var policy = new DefaultCommandExecutionPolicy();
        var descriptor = new RemoteCommandDescriptor { CommandKey = "server.start", DisplayName = "Start" };
        var request = new RemoteExecutionRequest { NodeId = "node-1", RequestedBy = "tester" };

        await policy.EnsureAllowedAsync(request, descriptor);
    }
}
