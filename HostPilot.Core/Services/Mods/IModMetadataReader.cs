using HostPilot.Core.Models.Mods;

namespace HostPilot.Core.Services.Mods;

public interface IModMetadataReader
{
    bool CanRead(string path);
    Task<ModImportCandidate?> TryReadAsync(string path, CancellationToken cancellationToken = default);
}
