namespace HostPilot.Remote.Transfer.Models;

public sealed class RemoteTransferValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
