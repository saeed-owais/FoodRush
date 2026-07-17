using FoodRush.Application.Abstractions.EventBus;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.DomainEvents;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.ApproveRestaurantDocument;

internal sealed class ApproveRestaurantDocumentCommandHandler
    (IRestaurantRepository restaurantRepository,
    IEventBus bus,
    IRestaurantQueries restaurantQueries,
    ILogger<ApproveRestaurantDocumentCommandHandler> logger)
    : ICommandHandler<ApproveRestaurantDocumentCommand>
{
    public async Task<Result> Handle(ApproveRestaurantDocumentCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);
        var documentId = new DocumentId(request.DocumentId);

        var restaurant =
            await restaurantRepository.GetWithDocumentsAsync(
                restaurantId,
                cancellationToken);

        if (restaurant == null)
        {
            logger.LogWarning(
                "Restaurant {RestaurantId} was not found while approving document {DocumentId}.",
                restaurantId,
                documentId);
            return Result.Failure(RestaurantErrors.NotFound(restaurantId));
        }

        var result = restaurant.ApproveDocument(documentId);

        if (result.IsFailure)
        {
            return result;
        }

        await PublishIntegrationEventsAsync(
            restaurant,
            documentId,
            cancellationToken);

        return Result.Success();
    }
    private async Task PublishIntegrationEventsAsync(
        Restaurant restaurant,
        DocumentId documentId,
        CancellationToken cancellationToken)
    {
        var ownerInfo = await restaurantQueries.GetOwnerInfoAsync(restaurant.Id, cancellationToken);

        await bus.Publish(
            new RestaurantDocumentApprovedIntegrationEvent(
                Guid.NewGuid(),
                restaurant.Id.Value,
                documentId.Value,
                restaurant.Name.Value,
                ownerInfo.Email,
                ownerInfo.Name),
            cancellationToken);

        var restaurantApprovedEvent = restaurant.DomainEvents
            .OfType<RestaurantApprovedDomainEvent>()
            .SingleOrDefault();
        if (restaurantApprovedEvent is not null)
        {
            await bus.Publish(new RestaurantApprovedIntegrationEvent(
                Guid.NewGuid(),
                restaurant.Id.Value,
                restaurant.Name.Value,
                ownerInfo.Email,
                ownerInfo.Name),
                cancellationToken);
        }
    }
}
