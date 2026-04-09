using System.Security.Cryptography;
using System.Text;
using HostPilot.Security.Contracts;

namespace HostPilot.Security.Services;

public sealed class HmacRequestSigner : IRequestSigner
{
    public string Sign(string secret, string message)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToHexString(bytes);
    }

    public bool Verify(string secret, string message, string signature)
    {
        var expected = Sign(secret, message);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(signature));
    }
}
