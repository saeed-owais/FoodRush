using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Common.Models;
using FoodRush.Domain.Common;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;

internal sealed class SearchRestaurantsQueryHandler
    (IRestaurantQueries restaurantQueries)
    : IQueryHandler<SearchRestaurantsQuery, PaginatedResponse<RestaurantDto>>
{
    public async Task<Result<PaginatedResponse<RestaurantDto>>> Handle(SearchRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var result = await restaurantQueries.SearchRestaurantsAsync(request, cancellationToken);

        return result;
    }
}
