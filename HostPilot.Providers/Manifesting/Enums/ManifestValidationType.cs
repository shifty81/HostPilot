namespace HostPilot.Providers.Manifesting.Enums;

public enum ManifestValidationType
{
    Required = 0,
    Min = 1,
    Max = 2,
    MinLength = 3,
    MaxLength = 4,
    Regex = 5,
    AllowedValues = 6,
    PortRange = 7,
    CompareNotEqual = 8,
    CompareEqual = 9
}
