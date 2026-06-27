
using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Restaurants.RegisterRestaurant;

public sealed record RegisterRestaurantCommand(
    string Name,
    double Latitude,
    double Longitude,
    double DeliveryRadiusKm)
    : ICommand<Guid>;