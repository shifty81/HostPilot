using HostPilot.Core.Execution.Contracts;
using HostPilot.Telemetry.Contracts;
using HostPilot.Telemetry.Services;
using Moq;
using Xunit;

namespace HostPilot.Tests.Telemetry;

public sealed class TelemetryBroadcasterTests
{
    [Fact]
    public async Task ExecutionEventBroadcaster_Publishes_To_Correct_Topic()
    {
        var broadcasterMock = new Mock<ITelemetryBroadcaster>();
        broadcasterMock
            .Setup(b => b.BroadcastAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = new ExecutionEventBroadcaster(broadcasterMock.Object);
        var evt = new ExecutionEvent(
            EventType: "WorkItemStarted",
            PlanId: "plan-1",
            WorkItemId: "item-1",
            NodeId: "node-1",
            Message: "Starting",
            TimestampUtc: DateTimeOffset.UtcNow);

        await sut.PublishAsync(evt, CancellationToken.None);

        broadcasterMock.Verify(
            b => b.BroadcastAsync("execution-events", evt, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task ExecutionEventBroadcaster_PassesThrough_CancellationToken()
    {
        var broadcasterMock = new Mock<ITelemetryBroadcaster>();
        CancellationToken capturedToken = default;
        broadcasterMock
            .Setup(b => b.BroadcastAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<string, object, CancellationToken>((_, _, ct) => capturedToken = ct)
            .Returns(Task.CompletedTask);

        var sut = new ExecutionEventBroadcaster(broadcasterMock.Object);
        using var cts = new CancellationTokenSource();

        await sut.PublishAsync(
            new ExecutionEvent("T", "p1", "w1", null, "msg", DateTimeOffset.UtcNow),
            cts.Token);

        Assert.Equal(cts.Token, capturedToken);
    }
}
