using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents;

public sealed record RestaurantApprovedDomainEvent(RestaurantId RestaurantId, Name RestaurantName)
    : DomainEvent;