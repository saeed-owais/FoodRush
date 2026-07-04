using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.SuspendRestaurant;

internal sealed class SuspendRestaurantCommandHandler
    (IRestaurantRepository restaurantRepository,
    ILogger<SuspendRestaurantCommandHandler> logger)
    : ICommandHandler<SuspendRestaurantCommand>
{
    public async Task<Result> Handle(SuspendRestaurantCommand request, CancellationToken cancellationToken)
    {
        RestaurantId restaurantId = new RestaurantId(request.RestaurantId);

        var restaurant =
            await restaurantRepository.GetByIdAsync(
                restaurantId,
                cancellationToken);

        if (restaurant == null)
        {
            logger.LogWarning(
                "Restaurant with ID {RestaurantId} not found.",
                request.RestaurantId);

            return Result.Failure(RestaurantErrors.NotFound(restaurantId));
        }

        var result = restaurant.Suspend(request.Reason);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Failed to suspend restaurant with ID {RestaurantId}. Reason: {Reason}",
                request.RestaurantId,
                result.Error);

            return result;
        }

        return result;
    }
}

