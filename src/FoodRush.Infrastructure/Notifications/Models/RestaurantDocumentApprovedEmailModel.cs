namespace FoodRush.Infrastructure.Notifications.Models;

public sealed record RestaurantDocumentApprovedEmailModel
{
    public string OwnerName { get; init; } = default!;

    public string RestaurantName { get; init; } = default!;
}
