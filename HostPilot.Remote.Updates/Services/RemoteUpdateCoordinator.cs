namespace HostPilot.Remote.Updates.Services;

using HostPilot.Remote.Contracts.Abstractions;
using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Updates.Abstractions;

public sealed class RemoteUpdateCoordinator : IRemoteUpdateCoordinator
{
    private readonly IRemoteUpdatePackageSource _packageSource;
    private readonly UpdateManifestValidator _validator;
    private readonly InMemoryUpdateStateStore _stateStore;

    public RemoteUpdateCoordinator(IRemoteUpdatePackageSource packageSource, UpdateManifestValidator validator, InMemoryUpdateStateStore stateStore)
    {
        _packageSource = packageSource;
        _validator = validator;
        _stateStore = stateStore;
    }

    public async Task RunPlanAsync(RemoteUpdatePlan plan, CancellationToken cancellationToken = default)
    {
        var manifest = await _packageSource.GetLatestAsync(plan.Channel, "win-x64", cancellationToken);
        if (manifest is null || !_validator.IsValid(manifest))
        {
            throw new InvalidOperationException("No valid update package manifest was found.");
        }

        foreach (var nodeId in plan.NodeIds)
        {
            _stateStore.Add(new RemoteUpdateProgress { PlanId = plan.PlanId, NodeId = nodeId, Stage = "Stage", Message = $"Staging {manifest.PackageId} {manifest.Version}" });
            _stateStore.Add(new RemoteUpdateProgress { PlanId = plan.PlanId, NodeId = nodeId, Stage = "Apply", Message = "Applying update" });
            _stateStore.Add(new RemoteUpdateProgress { PlanId = plan.PlanId, NodeId = nodeId, Stage = "Verify", Message = "Waiting for healthy heartbeat", HealthyAfterApply = true });
        }
    }
}
