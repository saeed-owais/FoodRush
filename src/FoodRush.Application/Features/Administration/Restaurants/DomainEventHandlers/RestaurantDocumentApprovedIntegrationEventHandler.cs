using FoodRush.Application.Abstractions.EventBus;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Domain.Restaurants.DomainEvents.Document;

namespace FoodRush.Application.Features.Administration.Restaurants.DomainEventHandlers;

internal class RestaurantDocumentApprovedIntegrationEventHandler
    (IEventBus eventBus,
    IRestaurantQueries restaurantQueries
    )
    : IDomainEventHandler<RestaurantDocumentApprovedDomainEvent>
{
    public async Task Handle(RestaurantDocumentApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
        var ownerInfo = await restaurantQueries.GetOwnerInfoAsync(notification.RestaurantId, cancellationToken);
        var integrationEvent = new RestaurantDocumentApprovedIntegrationEvent(
            notification.Id,
            notification.RestaurantId.Value,
            notification.DocumentId.Value,
            notification.RestaurantName,
            "saidewisali@gmail.com",
            ownerInfo.Name
        );

        await eventBus.Publish(integrationEvent, cancellationToken);
    }
}
