namespace FoodRush.Infrastructure.BackgroundJobs.Interfaces
{
    internal interface IRefreshTokenCleanupJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
