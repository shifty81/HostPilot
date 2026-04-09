namespace HostPilot.Security.Contracts;

public interface INodeTrustStore
{
    Task<NodeTrustRecord?> GetAsync(string nodeId, CancellationToken cancellationToken);
    Task SaveAsync(NodeTrustRecord record, CancellationToken cancellationToken);
    Task RevokeAsync(string nodeId, CancellationToken cancellationToken);
}

public interface INonceStore
{
    Task<bool> IsReplayAsync(string nodeId, string nonce, CancellationToken cancellationToken);
    Task RecordAsync(string nodeId, string nonce, CancellationToken cancellationToken);
}

public interface ISecretGenerator
{
    string GenerateSecret();
}

public interface IRequestSigner
{
    string Sign(string secret, string message);
    bool Verify(string secret, string message, string signature);
}
