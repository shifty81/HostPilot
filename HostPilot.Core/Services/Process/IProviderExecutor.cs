namespace HostPilot.Core.Services.Process;

public interface IProviderExecutor
{
    Task ExecuteInstallAsync(ProcessSupervisorProfile profile, IProgress<string> progress, CancellationToken cancellationToken);
    Task ExecuteUpdateAsync(ProcessSupervisorProfile profile, IProgress<string> progress, CancellationToken cancellationToken);
    Task ExecuteValidateAsync(ProcessSupervisorProfile profile, IProgress<string> progress, CancellationToken cancellationToken);
}
