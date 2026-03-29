namespace HostPilot.Core.Services.Process;

public sealed class ProviderExecutorRegistry
{
    private readonly Dictionary<string, IProviderExecutor> _executors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SteamCmd"] = new SteamCmdProviderExecutor()
    };

    public IProviderExecutor Get(string providerType)
    {
        if (_executors.TryGetValue(providerType, out var executor))
            return executor;
        throw new InvalidOperationException($"No provider executor registered for '{providerType}'.");
    }
}
