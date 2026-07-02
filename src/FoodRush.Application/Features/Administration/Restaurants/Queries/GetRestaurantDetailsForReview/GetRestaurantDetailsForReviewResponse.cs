namespace FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;

public sealed class RestaurantDetailsForReviewResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string OwnerName { get; init; } = default!;

    public string OwnerEmail { get; init; } = default!;

    public decimal Latitude { get; init; }

    public decimal Longitude { get; init; }

    public decimal DeliveryRadiusKm { get; init; }

    public List<RestaurantDocumentResponse> Documents { get; init; } = [];
}
