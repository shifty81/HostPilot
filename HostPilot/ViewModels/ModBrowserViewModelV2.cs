using HostPilot.Core.Models.Mods;

namespace HostPilot.ViewModels;

public sealed class ModBrowserViewModelV2
{
    public string SelectedProviderId { get; set; } = "steam-workshop";
    public string SelectedGameId { get; set; } = string.Empty;
    public string QueryText { get; set; } = string.Empty;

    public ModSearchQuery BuildQuery()
        => new()
        {
            ProviderId = SelectedProviderId,
            GameId = SelectedGameId,
            QueryText = QueryText,
            Page = 0,
            PageSize = 25
        };
}
