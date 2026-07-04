using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;

internal sealed class GetRestaurantDetailsForReviewQueryHandler
(
    IRestaurantQueries restaurantQueries,
    ILogger<GetRestaurantDetailsForReviewQueryHandler> logger
)
  : IQueryHandler<GetRestaurantDetailsForReviewQuery, RestaurantDetailsForReviewResponse>
{
    public async Task<Result<RestaurantDetailsForReviewResponse>> Handle(
            GetRestaurantDetailsForReviewQuery request,
            CancellationToken cancellationToken)
    {
        RestaurantId restaurantId = new RestaurantId(request.RestaurantId);

        var restaurant = await restaurantQueries
            .GetRestaurantDetailsForReviewAsync(
                restaurantId,
                cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning(
                "Restaurant with ID {RestaurantId} not found.",
                request.RestaurantId);

            return Result.Failure<RestaurantDetailsForReviewResponse>(
                RestaurantErrors.NotFound(restaurantId));
        }

        return restaurant;
    }
}
