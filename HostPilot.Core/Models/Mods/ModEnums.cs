namespace HostPilot.Core.Models.Mods;

public enum ModImportSourceType
{
    Unknown = 0,
    LocalFile = 1,
    LocalFolder = 2,
    ZipArchive = 3,
    BrowserProvider = 4,
    ManualPath = 5,
}

public enum ModPackageType
{
    Unknown = 0,
    SingleModFile = 1,
    Archive = 2,
    FolderPackage = 3,
    MultiModFolder = 4,
    MixedPayload = 5,
}

public enum ModInstallAction
{
    CopyAsIs = 0,
    ExtractArchive = 1,
    CopyDirectory = 2,
    Reject = 3,
    NeedsReview = 4,
}

public enum ModCompatibilityLevel
{
    Unknown = 0,
    Compatible = 1,
    Warning = 2,
    Incompatible = 3,
}
