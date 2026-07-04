using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;

public sealed record RestaurantDocumentResponse(
    Guid Id,
    DocumentType Type,
    DocumentStatus Status,
    string FileUrl);