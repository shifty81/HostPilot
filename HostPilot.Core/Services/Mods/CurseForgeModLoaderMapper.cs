namespace HostPilot.Core.Services.Mods;

public static class CurseForgeModLoaderMapper
{
    public static int? TryMap(string? loader)
    {
        if (string.IsNullOrWhiteSpace(loader))
            return null;

        return loader.Trim().ToLowerInvariant() switch
        {
            "forge" => 1,
            "cauldron" => 2,
            "liteloader" => 3,
            "fabric" => 4,
            "quilt" => 5,
            "neoforge" => 6,
            _ => null,
        };
    }
}
