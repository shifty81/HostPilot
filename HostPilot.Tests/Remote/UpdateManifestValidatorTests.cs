using HostPilot.Remote.Contracts.Models;
using HostPilot.Remote.Updates.Services;
using Xunit;

namespace HostPilot.Tests.Remote;

public sealed class UpdateManifestValidatorTests
{
    private readonly UpdateManifestValidator _validator = new();

    [Fact]
    public void Rejects_Unsigned_Manifest()
    {
        var manifest = new RemoteUpdatePackageManifest { PackageId = "agent", Version = "1.0.0", Sha256 = "abc", IsSigned = false };
        Assert.False(_validator.IsValid(manifest));
    }

    [Fact]
    public void Rejects_Manifest_With_Missing_Sha256()
    {
        var manifest = new RemoteUpdatePackageManifest { PackageId = "agent", Version = "1.0.0", IsSigned = true };
        Assert.False(_validator.IsValid(manifest));
    }

    [Fact]
    public void Accepts_Fully_Populated_Signed_Manifest()
    {
        var manifest = new RemoteUpdatePackageManifest
        {
            PackageId = "agent",
            Version = "2.1.0",
            Sha256 = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2",
            IsSigned = true,
        };
        Assert.True(_validator.IsValid(manifest));
    }
}
