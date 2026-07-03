using FoodRush.Domain.Restaurants.Enums;

namespace FoodRush.Application.Features.Restaurants.Onboarding;

public sealed class RestaurantOnboardingResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public RestaurantStatus Status { get; init; }

    public List<RestaurantOnboardingDocumentResponse> Documents { get; init; } = [];
}