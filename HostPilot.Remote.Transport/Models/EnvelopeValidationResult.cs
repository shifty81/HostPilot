namespace HostPilot.Remote.Transport;

public sealed class EnvelopeValidationResult
{
    public bool IsValid { get; init; }
    public string? FailureReason { get; init; }

    public static EnvelopeValidationResult Success() => new() { IsValid = true };
    public static EnvelopeValidationResult Failure(string reason) => new() { IsValid = false, FailureReason = reason };
}
