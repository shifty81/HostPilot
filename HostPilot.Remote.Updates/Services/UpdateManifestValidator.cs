namespace HostPilot.Remote.Updates.Services;

using HostPilot.Remote.Contracts.Models;

public sealed class UpdateManifestValidator
{
    public bool IsValid(RemoteUpdatePackageManifest manifest)
    {
        return !string.IsNullOrWhiteSpace(manifest.PackageId)
            && !string.IsNullOrWhiteSpace(manifest.Version)
            && !string.IsNullOrWhiteSpace(manifest.Sha256)
            && manifest.IsSigned;
    }
}
