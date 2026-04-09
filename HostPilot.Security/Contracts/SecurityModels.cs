namespace HostPilot.Security.Contracts;

public sealed record NodeBootstrapRequest(
    string NodeName,
    string MachineFingerprint,
    string BootstrapToken,
    IReadOnlyList<string> RequestedCapabilities);

public sealed record NodeTrustRecord(
    string NodeId,
    string NodeName,
    string SharedSecretId,
    string SharedSecret,
    DateTimeOffset IssuedUtc,
    DateTimeOffset ExpiresUtc,
    bool IsRevoked);

public sealed record SignedRequestEnvelope(
    string NodeId,
    string Nonce,
    DateTimeOffset TimestampUtc,
    string Signature,
    string PayloadJson);

public sealed record SessionToken(
    string Token,
    DateTimeOffset IssuedUtc,
    DateTimeOffset ExpiresUtc);
