using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Application.Features.Restaurants.Onboarding;

internal sealed class GetRestaurantOnboardingQueryHandler
    (IRestaurantQueries restaurantQueries)
    : IQueryHandler<GetRestaurantOnboardingQuery, RestaurantOnboardingResponse>
{
    public async Task<Result<RestaurantOnboardingResponse>> Handle(GetRestaurantOnboardingQuery request, CancellationToken cancellationToken)
    {
        RestaurantId restaurantId = new RestaurantId(request.RestaurantId);
        var restaurant =
            await restaurantQueries.GetMyRestaurantOnboardingQuery(
                restaurantId,
                cancellationToken);

        if (restaurant is null)
        {
            return Result.Failure<RestaurantOnboardingResponse>(
                RestaurantErrors.NotFound(restaurantId));
        }

        return restaurant;
    }
}
