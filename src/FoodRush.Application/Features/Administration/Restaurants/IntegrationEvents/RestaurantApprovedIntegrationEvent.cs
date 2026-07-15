namespace FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;

public sealed record RestaurantApprovedIntegrationEvent(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    string OwnerEmail,
    string OwnerName);
