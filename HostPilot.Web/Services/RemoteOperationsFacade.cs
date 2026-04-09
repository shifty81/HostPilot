namespace HostPilot.Web.Services;

using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Execution.Services;
using HostPilot.Remote.Transfer.Services;
using HostPilot.Remote.Updates.Abstractions;

public sealed class RemoteOperationsFacade
{
    private readonly RemoteCommandExecutionService _executionService;
    private readonly RemoteFileTransferService _transferService;
    private readonly IRemoteUpdateCoordinator _updateCoordinator;

    public RemoteOperationsFacade(
        RemoteCommandExecutionService executionService,
        RemoteFileTransferService transferService,
        IRemoteUpdateCoordinator updateCoordinator)
    {
        _executionService = executionService;
        _transferService = transferService;
        _updateCoordinator = updateCoordinator;
    }

    public Task<RemoteExecutionResult> ExecuteAsync(RemoteExecutionRequest request, CancellationToken cancellationToken = default)
        => _executionService.ExecuteAsync(request, cancellationToken);

    public Task RunUpdatePlanAsync(RemoteUpdatePlan plan, CancellationToken cancellationToken = default)
        => _updateCoordinator.RunPlanAsync(plan, cancellationToken);
}
