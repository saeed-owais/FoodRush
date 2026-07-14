namespace FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;

public sealed record RestaurantDocumentApprovedIntegrationEvent(
    Guid Id,
    Guid RestaurantId,
    Guid DocumentId,
    string RestaurantName,
    string OwnerEmail,
    string OwnerName);