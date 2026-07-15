namespace FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;

public sealed record RestaurantDocumentRejectedIntegrationEvent(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    Guid DocumentId,
    string DocumentName,
    string OwnerName,
    string OwnerEmail,
    string Reason);