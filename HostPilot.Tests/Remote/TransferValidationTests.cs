using HostPilot.Remote.Transfer.Models;
using Xunit;

namespace HostPilot.Tests.Remote;

public sealed class TransferValidationTests
{
    [Fact]
    public void Can_Construct_Valid_Result()
    {
        var result = new RemoteTransferValidationResult { IsValid = true, Message = "ok" };
        Assert.True(result.IsValid);
        Assert.Equal("ok", result.Message);
    }

    [Fact]
    public void Can_Construct_Invalid_Result()
    {
        var result = new RemoteTransferValidationResult { IsValid = false, Message = "Provider not found." };
        Assert.False(result.IsValid);
    }
}
