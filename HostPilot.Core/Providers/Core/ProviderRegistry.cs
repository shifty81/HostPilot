using HostPilot.Core.Providers.Abstractions;
using HostPilot.Core.Providers.Adapters;

namespace HostPilot.Core.Providers.Core;

public sealed class ProviderRegistry
{
    private readonly Dictionary<string, IGameProviderAdapter> _providers;

    public ProviderRegistry(IEnumerable<IGameProviderAdapter>? providers = null)
    {
        providers ??=
        [
            new ArkSurvivalAscendedProvider(),
            new RustProvider(),
            new ValheimProvider(),
            new SourceEngineProvider(),
            new PalworldProvider()
        ];

        _providers = providers.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<IGameProviderAdapter> All => _providers.Values.ToList().AsReadOnly();

    public IGameProviderAdapter GetRequired(string providerId)
        => _providers.TryGetValue(providerId, out var provider)
            ? provider
            : throw new KeyNotFoundException($"Provider '{providerId}' was not registered.");
}
