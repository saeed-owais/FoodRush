using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.ReactivateRestaurant;

public sealed record ReactivateRestaurantCommand(Guid RestaurantId) : ICommand;
