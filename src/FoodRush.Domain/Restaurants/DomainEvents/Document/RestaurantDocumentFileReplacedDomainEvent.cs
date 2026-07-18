using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentFileReplacedDomainEvent(
    RestaurantId RestaurantId,
    DocumentId DocumentId,
    PublicId OldDocumentPublicId,
    PublicId NewDocumentPublicId) : DomainEvent;