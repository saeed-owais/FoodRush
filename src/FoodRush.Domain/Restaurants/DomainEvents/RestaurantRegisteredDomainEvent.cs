using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents;

public sealed record RestaurantRegisteredDomainEvent(Guid Id, RestaurantId RestaurantId)
    : DomainEvent(Id);