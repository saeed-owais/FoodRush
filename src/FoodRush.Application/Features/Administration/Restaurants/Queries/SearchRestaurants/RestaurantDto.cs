namespace FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;

public sealed record RestaurantDto(
    Guid Id,
    string Name,
    string Status,
    DateTime SubmittedAt);

