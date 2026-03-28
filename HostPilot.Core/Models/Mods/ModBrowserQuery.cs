namespace HostPilot.Core.Models.Mods;

public sealed class ModBrowserQuery
{
    public string? SearchText { get; set; }
    public string? GameVersion { get; set; }
    public string? Loader { get; set; }
    public string? LoaderType
    {
        get => Loader;
        set => Loader = value;
    }
    public string? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
