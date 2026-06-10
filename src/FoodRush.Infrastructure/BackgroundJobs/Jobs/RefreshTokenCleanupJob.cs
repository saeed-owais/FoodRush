using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Infrastructure.BackgroundJobs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.BackgroundJobs.Jobs
{
    internal sealed class RefreshTokenCleanupJob : IRefreshTokenCleanupJob
    {
        private readonly IApplicationDbContext _dbContext;
        public RefreshTokenCleanupJob(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            DateTime utcNow = DateTime.UtcNow;

            DateTime cutoffDate =
                utcNow.AddDays(-30);

            await _dbContext.RefreshTokens
                .Where(rt =>
                    rt.ExpiresAt < utcNow ||

                    (rt.RevokedAt != null &&
                     rt.RevokedAt < cutoffDate) ||

                    (rt.UsedAt != null &&
                     rt.UsedAt < cutoffDate)
                )
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}
