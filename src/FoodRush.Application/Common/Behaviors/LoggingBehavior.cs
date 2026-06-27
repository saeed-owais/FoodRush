using FoodRush.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FoodRush.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    private const long SlowRequestThresholdMs = 1000;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        Stopwatch stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Processing request {RequestName}", requestName);

        try
        {
            TResponse response = await next();

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected {RequestName} took {ElapsedMilliseconds}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            if (response is Result result)
            {
                if (result.IsSuccess)
                {
                    _logger.LogInformation(
                        "Completed request {RequestName} in {ElapsedMilliseconds}ms",
                        requestName,
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogWarning(
                        "Request {RequestName} failed in {ElapsedMilliseconds}ms with error {@Error}",
                        requestName,
                        stopwatch.ElapsedMilliseconds,
                        result.Error);
                }

                return response;
            }

            _logger.LogInformation(
                "Completed request {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Request {RequestName} crashed after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}