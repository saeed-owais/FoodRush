using FoodRush.Domain.Common;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.DomainEvents.Document;

public sealed record RestaurantDocumentRejectedDomainEvent(
    RestaurantId RestaurantId,
    Name Name,
    DocumentId DocumentId,
    string DocumentName,
    string Reason)
    : DomainEvent;