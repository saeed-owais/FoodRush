using FoodRush.Application.Abstractions.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace FoodRush.Infrastructure.Authentication
{
    internal sealed class RefreshTokenHasher : IRefreshTokenHasher
    {
        public string Hash(string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            byte[] x = SHA256.HashData(Encoding.UTF8.GetBytes(token));

            return Convert.ToBase64String(x);
        }

        public bool Verify(string rawToken, string hash)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(hash);

            byte[] rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));

            byte[] storedHash = Convert.FromBase64String(hash);

            return CryptographicOperations.FixedTimeEquals(rawHash, storedHash);
        }
    }
}
