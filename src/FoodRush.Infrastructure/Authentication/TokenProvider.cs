using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Common.Settings;
using FoodRush.Application.Features.Authentication;
using FoodRush.Domain.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FoodRush.Infrastructure.Authentication
{
    internal sealed class TokenProvider(IOptions<JwtSettings> jwtSettings) : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public TokenResult GenerateToken(User user, IEnumerable<string> roles)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = [

                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new ("security_stamp", user.SecurityStamp)
            ];

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = signingCredentials,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            };

            var tokenHandler = new JsonWebTokenHandler();

            string token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResult(
                token,
                tokenDescriptor.Subject.FindFirst(JwtRegisteredClaimNames.Jti)?.Value,
                tokenDescriptor.Expires.Value);

        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(
               RandomNumberGenerator.GetBytes(64));
        }
    }
}
