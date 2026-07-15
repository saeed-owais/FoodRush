using FoodRush.Application.Abstractions.EventBus;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Domain.Restaurants.DomainEvents;

namespace FoodRush.Application.Features.Administration.Restaurants.DomainEventHandlers;

internal sealed class RestaurantApprovedDomainEventHandler
    (IEventBus bus,
    IRestaurantQueries restaurantQueries)
    : IDomainEventHandler<RestaurantApprovedDomainEvent>
{
    public async Task Handle(
        RestaurantApprovedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var ownerInfo = await restaurantQueries.GetOwnerInfoAsync(
            notification.RestaurantId,
            cancellationToken);

        await bus.Publish(
            new RestaurantApprovedIntegrationEvent(
                notification.Id,
                notification.RestaurantId.Value,
                notification.RestaurantName.Value,
                ownerInfo.Email,
                ownerInfo.Name),
            cancellationToken);
    }
}
