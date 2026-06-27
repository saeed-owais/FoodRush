using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentReplacedDomainEvent(
    Guid Id,
    RestaurantId RestaurantId,
    PublicId OldDocumentPublicId,
    PublicId NewDocumentPublicId
) : DomainEvent(Id);
