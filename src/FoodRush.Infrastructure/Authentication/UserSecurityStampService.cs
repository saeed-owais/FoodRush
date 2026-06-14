using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace FoodRush.Infrastructure.Authentication;

internal class UserSecurityStampService
    (HybridCache cache,
    IApplicationDbContext dbContext) : IUserSecurityStampService
{
    private static string Key(Guid userId) => $"securitystamp:{userId}";

    public async Task<string?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
        => await cache.GetOrCreateAsync(
            Key(userId),
            async token =>
            {
                return await dbContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.SecurityStamp)
                    .FirstOrDefaultAsync(token);
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
            },
            cancellationToken: cancellationToken);

    public async Task SetAsync(Guid userId, string securityStamp, CancellationToken cancellationToken = default)
        => await cache.SetAsync(Key(userId), securityStamp, cancellationToken: cancellationToken);

}
