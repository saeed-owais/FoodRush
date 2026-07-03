using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.SuspendRestaurant;

public sealed record SuspendRestaurantCommand(
    Guid RestaurantId,
    string Reason) : ICommand;