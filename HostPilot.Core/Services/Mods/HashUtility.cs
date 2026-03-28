using System.Security.Cryptography;

namespace HostPilot.Core.Services.Mods;

public static class HashUtility
{
    public static string ComputeSha256(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }
}
