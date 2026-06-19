using MediatR;

namespace FoodRush.Domain.Common;

public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
}