using HostPilot.Core.Models.FirstRun;

namespace HostPilot.Core.Services.Startup;

public interface IFirstRunStateStore
{
    FirstRunWizardState Load();
    void Save(FirstRunWizardState state);
}
