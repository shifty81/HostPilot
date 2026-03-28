using System;
using System.Collections.Generic;
using System.Linq;
using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public sealed class ModProviderRegistry
{
    private readonly Dictionary<ModProviderType, IModProvider> _providers;

    public ModProviderRegistry(IEnumerable<IModProvider> providers)
    {
        _providers = providers.ToDictionary(x => x.ProviderType, x => x);
    }

    public IModProvider? GetProvider(ModProviderType type)
        => _providers.TryGetValue(type, out var provider) ? provider : null;

    public IReadOnlyList<IModProvider> GetProviders(IEnumerable<ModProviderType> types)
        => types.Select(GetProvider).Where(x => x is not null).Cast<IModProvider>().ToList();

    public IReadOnlyList<IModProvider> GetAll() => _providers.Values.ToList();
}
