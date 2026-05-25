using FoodRush.Infrastructure.BackgroundJobs.Interfaces;
using FoodRush.Infrastructure.BackgroundJobs.Jobs;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FoodRush.Infrastructure.BackgroundJobs;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddHangfire(configuration =>
        {
            configuration
                .SetDataCompatibilityLevel(
                    CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout =
                            TimeSpan.FromMinutes(5),

                        SlidingInvisibilityTimeout =
                            TimeSpan.FromMinutes(5),

                        QueuePollInterval =
                            TimeSpan.Zero,

                        UseRecommendedIsolationLevel = true,

                        DisableGlobalLocks = true
                    });
        });

        services.AddHangfireServer();

        services.AddScoped<IRefreshTokenCleanupJob,
            RefreshTokenCleanupJob>();

        return services;
    }

    public static void UseBackgroundJobs(
        this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire");

        RecurringJob.AddOrUpdate<IRefreshTokenCleanupJob>(
            "refresh-token-cleanup",
            job => job.ExecuteAsync(default),
            Cron.Daily);
    }
}
