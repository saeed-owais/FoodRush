using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Abstractions.Messaging;

public interface IQuery<TResponse>
    : IRequest<Result<TResponse>>
{
}
