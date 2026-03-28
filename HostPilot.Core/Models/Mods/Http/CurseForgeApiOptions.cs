namespace HostPilot.Core.Models.Mods.Http;

public sealed class CurseForgeApiOptions
{
    public string BaseUrl { get; set; } = "https://api.curseforge.com";
    public string? ApiKey { get; set; }
    public int GameId { get; set; } = 432; // Minecraft by default.
    public int PageSizeLimit { get; set; } = 50;
}
