using FoodRush.Infrastructure.BackgroundJobs.Interfaces;
using Hangfire;

namespace FoodRush.Infrastructure.BackgroundJobs;

internal static class BackgroundJobsConfiguration
{
    public static void RegisterRecurringJobs()
    {
        RecurringJob.AddOrUpdate<IRefreshTokenCleanupJob>(
            BackgroundJobNames.RefreshTokenCleanup,
            job => job.ExecuteAsync(default),
            Cron.Monthly);
    }
}
