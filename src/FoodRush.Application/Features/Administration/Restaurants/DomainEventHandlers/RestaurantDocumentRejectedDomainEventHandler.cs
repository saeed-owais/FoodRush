using FoodRush.Application.Abstractions.EventBus;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Domain.Restaurants.DomainEvents.Document;

namespace FoodRush.Application.Features.Administration.Restaurants.DomainEventHandlers;

internal sealed class RestaurantDocumentRejectedDomainEventHandler
    (IEventBus bus,
    IRestaurantQueries restaurantQueries)
    : IDomainEventHandler<RestaurantDocumentRejectedDomainEvent>
{
    public async Task Handle(
        RestaurantDocumentRejectedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var ownerInfo = await restaurantQueries.GetOwnerInfoAsync(
            notification.RestaurantId,
            cancellationToken);

        await bus.Publish(
            new RestaurantDocumentRejectedIntegrationEvent(
                notification.Id,
                notification.RestaurantId.Value,
                notification.Name.Value,
                notification.DocumentId.Value,
                notification.DocumentName,
                ownerInfo.Name,
                ownerInfo.Email,
                notification.Reason),
            cancellationToken);
    }
}
