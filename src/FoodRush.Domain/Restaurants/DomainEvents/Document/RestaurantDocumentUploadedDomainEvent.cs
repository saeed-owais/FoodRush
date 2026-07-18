using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentUploadedDomainEvent
    (
    RestaurantId RestaurantId,
    DocumentId DocumentId,
    DocumentType DocumentType)
    : DomainEvent;