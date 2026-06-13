namespace FoodRush.Application.Abstractions.Authentication;

public interface IRefreshTokenService
{
    public Task RevokeAllAsync(
        Guid userId,
        string? revokedByIp,
        DateTime utcNow,
        CancellationToken cancellationToken = default);
}
