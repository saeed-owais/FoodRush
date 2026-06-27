using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentResubmittedDomainEvent
    (Guid Id, RestaurantId RestaurantId, DocumentId DocumentId)
    : DomainEvent(Id);