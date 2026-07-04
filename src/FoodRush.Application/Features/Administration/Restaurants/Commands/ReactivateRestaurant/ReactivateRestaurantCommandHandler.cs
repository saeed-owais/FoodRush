using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.ReactivateRestaurant;

internal sealed class ReactivateRestaurantCommandHandler
    (IRestaurantRepository restaurantRepository,
    ILogger<ReactivateRestaurantCommandHandler> logger
    ) : ICommandHandler<ReactivateRestaurantCommand>
{
    public async Task<Result> Handle(
        ReactivateRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);

        var restaurant = await restaurantRepository.GetByIdAsync(
            restaurantId,
            cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning(
                "Restaurant {RestaurantId} was not found while reactivating.",
                restaurantId);

            return Result.Failure(
                RestaurantErrors.NotFound(restaurantId));
        }

        var result = restaurant.Reactivate();

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Failed to reactivate restaurant {RestaurantId}. Error: {Error}",
                restaurantId,
                result.Error);

            return result;
        }

        logger.LogInformation(
            "Restaurant {RestaurantId} reactivated successfully.",
            restaurantId);

        return Result.Success();
    }
}
