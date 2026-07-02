using FoodRush.Application.Common.Models;
using FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;
using FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Application.Abstractions.Persistence.Queries;

public interface IRestaurantQueries
{
    Task<PaginatedResponse<RestaurantDto>> SearchRestaurantsAsync(
        SearchRestaurantsQuery request,
        CancellationToken cancellationToken);

    Task<RestaurantDetailsForReviewResponse?> GetRestaurantDetailsForReviewAsync(
        RestaurantId restaurantId,
        CancellationToken cancellationToken);

}