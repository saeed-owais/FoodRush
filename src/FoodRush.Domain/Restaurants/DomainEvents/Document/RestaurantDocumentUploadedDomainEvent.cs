using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentUploadedDomainEvent
    (Guid Id, RestaurantId RestaurantId, DocumentId DocumentId, DocumentType DocumentType)
    : IDomainEvent;