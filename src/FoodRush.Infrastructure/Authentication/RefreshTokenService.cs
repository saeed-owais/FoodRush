using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Authentication;

internal sealed class RefreshTokenService
    (IApplicationDbContext dbContext) : IRefreshTokenService
{
    public async Task RevokeAllAsync(Guid userId, string? revokedByIp, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens
            .Where(rt =>
                rt.UserId == userId &&
                rt.RevokedAt == null &&
                rt.UsedAt == null &&
                rt.ExpiresAt > utcNow)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(rt => rt.RevokedAt, utcNow)
                    .SetProperty(rt => rt.RevokedByIp, revokedByIp),
                cancellationToken);
    }
}
