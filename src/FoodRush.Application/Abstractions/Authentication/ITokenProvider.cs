using FoodRush.Domain.Entities.Identity;

namespace FoodRush.Application.Abstractions.Authentication
{
    public interface ITokenProvider
    {
        string GenerateToken(User user, IEnumerable<string> roles);
        string GenerateRefreshToken();
    }
}
