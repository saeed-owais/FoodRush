using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Restaurants.SubmitForReview;

internal sealed class SubmitForReviewCommandHandler
    (
    IRestaurantRepository repository,
    ILogger<SubmitForReviewCommandHandler> logger)
    : ICommandHandler<SubmitForReviewCommand>
{
    public async Task<Result> Handle(SubmitForReviewCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);

        var restaurant = await repository.GetWithDocumentsAsync(restaurantId, cancellationToken);

        if (restaurant == null)
        {
            logger.LogWarning("Restaurant with ID {RestaurantId} not found", restaurantId);
            return Result.Failure(RestaurantErrors.NotFound(restaurantId));
        }

        var result = restaurant.SubmitForReview();
        if (result.IsFailure)
        {
            logger.LogWarning("Failed to submit restaurant for review: {Error}", result.Error);
            return Result.Failure(result.Error);
        }

        return Result.Success();
    }
}
