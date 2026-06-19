using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
