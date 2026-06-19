using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Abstractions.Messaging;

public interface IBaseCommand
{
}

public interface ICommand : IBaseCommand, IRequest<Result>
{
}

public interface ICommand<TResponse> : IBaseCommand, IRequest<Result<TResponse>>
{
}


