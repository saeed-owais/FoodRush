using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.RejectRestaurantDocument;

internal sealed class RejectRestaurantDocumentCommandHandler
    (IRestaurantRepository restaurantRepository,
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

        return Result.Success();
    }
}
