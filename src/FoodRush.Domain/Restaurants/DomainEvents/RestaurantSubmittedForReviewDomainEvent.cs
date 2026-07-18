using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents;

public sealed record RestaurantSubmittedForReviewDomainEvent(RestaurantId RestaurantId)
    : DomainEvent;