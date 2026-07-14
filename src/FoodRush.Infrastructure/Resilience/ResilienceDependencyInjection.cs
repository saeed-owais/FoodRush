using Amazon.S3;
using FoodRush.Infrastructure.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Net;
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

        services.AddResiliencePipeline(
            PipelineNames.SendEmail,
            (builder, context) =>
            {
                ILogger logger = context.ServiceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("SendEmailPipeline");

                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,

                    Delay = TimeSpan.FromSeconds(2),

                    BackoffType = DelayBackoffType.Exponential,

                    UseJitter = true,

                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutRejectedException>()
                        .Handle<EmailSendFailedException>(ex =>
                            ex.StatusCode == HttpStatusCode.TooManyRequests ||
                            ex.StatusCode == HttpStatusCode.InternalServerError ||
                            ex.StatusCode == HttpStatusCode.BadGateway ||
                            ex.StatusCode == HttpStatusCode.ServiceUnavailable ||
                            ex.StatusCode == HttpStatusCode.GatewayTimeout),

                    OnRetry = args =>
                    {
                        logger.LogWarning(
                            args.Outcome.Exception,
                            "Retry attempt {Attempt} sending email.",
                            args.AttemptNumber + 1);

                        return default;
                    }
                });

                builder.AddTimeout(TimeSpan.FromSeconds(15));

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,

                    SamplingDuration = TimeSpan.FromMinutes(1),

                    MinimumThroughput = 10,

                    BreakDuration = TimeSpan.FromSeconds(30),

                    OnOpened = args =>
                    {
                        logger.LogError(
                            "Email circuit breaker opened.");

                        return default;
                    },

                    OnHalfOpened = args =>
                    {
                        logger.LogInformation(
                            "Email circuit breaker is half-open.");

                        return default;
                    },

                    OnClosed = args =>
                    {
                        logger.LogInformation(
                            "Email circuit breaker closed.");

                        return default;
                    }
                });
            });

        return services;
    }

}

