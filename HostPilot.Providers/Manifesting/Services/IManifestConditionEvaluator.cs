using HostPilot.Providers.Manifesting.Contracts;
using HostPilot.Providers.Manifesting.Models;

namespace HostPilot.Providers.Manifesting.Services;

public interface IManifestConditionEvaluator
{
    bool Evaluate(ManifestConditionGroup? group, ManifestEvaluationContext context);
}
