using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentFileReplacedDomainEvent(
    Guid EventId,
    RestaurantId RestaurantId,
    DocumentId DocumentId,
    PublicId OldDocumentPublicId,
    PublicId NewDocumentPublicId) : DomainEvent(EventId);