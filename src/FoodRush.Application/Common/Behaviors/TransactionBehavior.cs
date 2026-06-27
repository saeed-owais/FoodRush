using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Common.Behaviors;

internal sealed class TransactionBehavior<TRequest, TResponse>
    (IApplicationDbContext context)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        if (response.IsSuccess)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }
}
