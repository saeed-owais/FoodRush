namespace FoodRush.Infrastructure.Notifications.Models;

public sealed record RestaurantDocumentRejectedEmailModel(
    string OwnerName,
    string RestaurantName,
    string DocumentName,
    string Reason);
