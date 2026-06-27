using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents;

public sealed record RestaurantReactivatedDomainEvent(Guid Id, RestaurantId RestaurantId)
    : DomainEvent(Id);