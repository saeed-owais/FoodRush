using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.ApproveRestaurantDocument;

internal sealed class ApproveRestaurantDocumentCommandHandler
    (IRestaurantRepository restaurantRepository,
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

        return Result.Success();
    }
}
