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
<<<<<<< HEAD
            Cron.Monthly);
=======
            Cron.Daily);
>>>>>>> b4f2e5f0f1a7ae20086cf5f8dde65b9d35b6f639
    }
}
