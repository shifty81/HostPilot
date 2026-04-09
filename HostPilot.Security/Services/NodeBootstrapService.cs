using HostPilot.Security.Contracts;

namespace HostPilot.Security.Services;

public sealed class NodeBootstrapService
{
    private readonly INodeTrustStore _trustStore;
    private readonly ISecretGenerator _secretGenerator;

    public NodeBootstrapService(
        INodeTrustStore trustStore,
        ISecretGenerator secretGenerator)
    {
        _trustStore = trustStore;
        _secretGenerator = secretGenerator;
    }

    public async Task<NodeTrustRecord> BootstrapAsync(
        NodeBootstrapRequest request,
        string approvedBootstrapToken,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(request.BootstrapToken, approvedBootstrapToken, StringComparison.Ordinal))
            throw new InvalidOperationException("Bootstrap token was invalid.");

        var record = new NodeTrustRecord(
            Guid.NewGuid().ToString("N"),
            request.NodeName,
            Guid.NewGuid().ToString("N"),
            _secretGenerator.GenerateSecret(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(30),
            false);

        await _trustStore.SaveAsync(record, cancellationToken);
        return record;
    }
}
