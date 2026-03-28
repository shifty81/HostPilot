using SteamServerTool.Core.Models.Mods;

namespace SteamServerTool.Core.Services.Mods;

public interface IModMetadataReader
{
    bool CanRead(string path);
    Task<ModImportCandidate?> TryReadAsync(string path, CancellationToken cancellationToken = default);
}
