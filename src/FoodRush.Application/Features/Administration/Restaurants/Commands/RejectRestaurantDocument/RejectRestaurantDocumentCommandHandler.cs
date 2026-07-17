using FoodRush.Application.Abstractions.EventBus;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.RejectRestaurantDocument;

internal sealed class RejectRestaurantDocumentCommandHandler
    (IRestaurantRepository restaurantRepository,
    IEventBus bus,
    IRestaurantQueries restaurantQueries,
     ILogger<RejectRestaurantDocumentCommandHandler> logger)
    : ICommandHandler<RejectRestaurantDocumentCommand>
{
    public async Task<Result> Handle(RejectRestaurantDocumentCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);
        var documentId = new DocumentId(request.DocumentId);

        var restaurant =
            await restaurantRepository.GetWithDocumentByIdAsync(
                restaurantId,
                documentId,
                cancellationToken);

        if (restaurant == null)
        {
            logger.LogWarning(
                "Restaurant {RestaurantId} was not found while rejecting document {DocumentId}.",
                restaurantId,
                documentId);
            return Result.Failure(RestaurantErrors.NotFound(restaurantId));
        }

        var result = restaurant.RejectDocument(documentId, request.Reason);

        if (result.IsFailure)
        {
            return result;
        }

        await PublishIntegrationEventsAsync(
            restaurant,
            documentId,
            request.Reason,
            cancellationToken);

        return Result.Success();
    }
    private async Task PublishIntegrationEventsAsync(
        Restaurant restaurant,
        DocumentId documentId,
        string reason,
        CancellationToken cancellationToken)
    {
        var ownerInfo = await restaurantQueries.GetOwnerInfoAsync(restaurant.Id, cancellationToken);
        var document = restaurant.Documents.FirstOrDefault();
        await bus.Publish(
            new RestaurantDocumentRejectedIntegrationEvent(
                Guid.NewGuid(),
                restaurant.Id.Value,
                restaurant.Name.Value,
                documentId.Value,
                document.Type.ToString(),
                ownerInfo.Name,
                ownerInfo.Email,
                reason),
            cancellationToken);
    }
}
