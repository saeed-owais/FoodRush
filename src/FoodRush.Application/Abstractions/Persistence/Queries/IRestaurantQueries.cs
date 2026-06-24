using FoodRush.Application.Common.Models;
using FoodRush.Application.Features.Administration.Restaurants.Queries;

namespace FoodRush.Application.Abstractions.Persistence.Queries;

public interface IRestaurantQueries
{
    Task<PaginatedResponse<RestaurantDto>> SearchRestaurantsAsync(
        SearchRestaurantsDto request,
        CancellationToken cancellationToken);
}