namespace HostPilot.Core.Models.Mods;

public sealed class ModSearchQuery
{
    public string ProviderId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string QueryText { get; set; } = string.Empty;
    public int Page { get; set; }
    public int PageSize { get; set; } = 25;
}
