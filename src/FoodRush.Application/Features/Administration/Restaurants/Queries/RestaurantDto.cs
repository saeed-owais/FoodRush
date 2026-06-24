namespace FoodRush.Application.Features.Administration.Restaurants.Queries;

public sealed record RestaurantDto(
    Guid Id,
    string Name,
    string Status,
    DateTime SubmittedAt);

