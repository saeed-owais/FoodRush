using FoodRush.Application.Features.Authentication;
using FoodRush.Domain.Entities.Identity;

namespace FoodRush.Application.Abstractions.Authentication
{
    public interface ITokenProvider
    {
        TokenResult GenerateToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
        string GenerateRefreshToken();
    }
}
