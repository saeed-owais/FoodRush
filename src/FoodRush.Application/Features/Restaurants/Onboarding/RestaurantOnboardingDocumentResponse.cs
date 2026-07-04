using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

namespace FoodRush.Application.Features.Restaurants.Onboarding;

public sealed class RestaurantOnboardingDocumentResponse
{
    public Guid Id { get; init; }
    public DocumentType Type { get; init; }
    public DocumentStatus Status { get; init; }
    public string FileUrl { get; init; } = default!;
    public string? RejectionReason { get; init; }
}