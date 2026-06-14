namespace FoodRush.Application.Abstractions.Authentication;

public interface IRefreshTokenHasher
{
    string Hash(string token);
    bool Verify(string rawToken, string hash);
}
