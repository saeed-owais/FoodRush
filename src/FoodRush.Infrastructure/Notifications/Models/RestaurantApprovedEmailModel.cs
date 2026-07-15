namespace FoodRush.Infrastructure.Notifications.Models;

public sealed record RestaurantApprovedEmailModel(
string OwnerName,
string RestaurantName);