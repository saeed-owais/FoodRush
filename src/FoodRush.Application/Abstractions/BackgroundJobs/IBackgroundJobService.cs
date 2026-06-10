using System.Linq.Expressions;

namespace FoodRush.Application.Abstractions.BackgroundJobs;

public interface IBackgroundJobService
{
    string Enqueue<T>(Expression<Func<T, Task>> methodCall);

    string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

    void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression);

    void RemoveRecurringJob(string recurringJobId);

    bool Delete(string jobId);

    string ContinueWith<T>(string parentJobId, Expression<Func<T, Task>> methodCall);

}
