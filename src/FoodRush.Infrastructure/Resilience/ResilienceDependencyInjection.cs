using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
namespace FoodRush.Infrastructure.Resilience;

internal static class ResilienceDependencyInjection
{
    public static IServiceCollection AddResilience(
        this IServiceCollection services)
    {
        services.AddResiliencePipeline(
            PipelineNames.R2Upload,
            (builder, context) =>
            {
                var logger = context.ServiceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("R2UploadPipeline");

                builder.AddRetry(
                    new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(1),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,

                        ShouldHandle = new PredicateBuilder()
                            .Handle<AmazonS3Exception>()
                            .Handle<TimeoutRejectedException>(),

                        OnRetry = args =>
                        {
                            logger.LogWarning(
                            "Retry attempt {AttemptNumber} for Cloudflare R2 upload",
                            args.AttemptNumber);

                            return default;
                        }
                    });

                builder.AddTimeout(
                    TimeSpan.FromSeconds(30));
            });

        services.AddResiliencePipeline(PipelineNames.R2Remove,
            (builder, context) =>
            {
                var logger = context.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("R2RemovePipeline");

                builder.AddRetry(
                    new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                        Delay = TimeSpan.FromSeconds(1),
                        UseJitter = true,
                        OnRetry = args =>
                        {
                            logger.LogWarning(
                            "Retry attempt {AttemptNumber} for Cloudflare R2 remove",
                            args.AttemptNumber);
                            return default;
                        },
                        ShouldHandle = new PredicateBuilder()
                            .Handle<AmazonS3Exception>()
                            .Handle<TimeoutRejectedException>()
                    });

                builder.AddTimeout(
                    TimeSpan.FromSeconds(10));
            });

        return services;
    }
}