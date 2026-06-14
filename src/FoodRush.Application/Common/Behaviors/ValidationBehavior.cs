using FluentValidation;
using FoodRush.Application.Common.Errors;
using MediatR;
using System.Reflection;

namespace FoodRush.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> _validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToArray();

        if (!errors.Any())
            return await next();

        return CreateValidationFailure(new ValidationError(errors));
    }

    private static TResponse CreateValidationFailure(ValidationError error)
    {
        // Result<TValue>
        if (typeof(TResponse).IsGenericType)
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            return (TResponse)typeof(Result<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(Result<object>.ValidationFailure),
                    BindingFlags.Public | BindingFlags.Static)!
                .Invoke(null, [error])!;
        }

        // Result
        return (TResponse)Result.Failure(error);
    }
}