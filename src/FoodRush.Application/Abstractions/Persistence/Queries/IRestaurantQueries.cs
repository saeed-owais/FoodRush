using FoodRush.Application.Common.Models;
using FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;

namespace FoodRush.Application.Abstractions.Persistence.Queries;

public interface IRestaurantQueries
{
    Task<PaginatedResponse<RestaurantDto>> SearchRestaurantsAsync(
        SearchRestaurantsQuery request,
        CancellationToken cancellationToken);
}