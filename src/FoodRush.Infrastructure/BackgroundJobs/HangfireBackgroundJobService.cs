using FoodRush.Application.Abstractions.BackgroundJobs;
using Hangfire;
using System.Linq.Expressions;

namespace FoodRush.Infrastructure.BackgroundJobs;

internal sealed class HangfireBackgroundJobService
    : IBackgroundJobService
{
    public void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression)
        => RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);

    public bool Delete(string jobId)
        => BackgroundJob.Delete(jobId);

    public string ContinueWith<T>(string parentJobId, Expression<Func<T, Task>> methodCall)
        => BackgroundJob.ContinueJobWith(parentJobId, methodCall);

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    public void RemoveRecurringJob(string recurringJobId)
        => RecurringJob.RemoveIfExists(recurringJobId);

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);


}
