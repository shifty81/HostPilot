using System.Security.Cryptography;
using HostPilot.Security.Contracts;

namespace HostPilot.Security.Services;

public sealed class Base64SecretGenerator : ISecretGenerator
{
    public string GenerateSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}
