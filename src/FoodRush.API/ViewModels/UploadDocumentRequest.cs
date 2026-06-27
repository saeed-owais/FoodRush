using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

namespace FoodRush.API.ViewModels;

public sealed record UploadDocumentRequest(
    DocumentType DocumentType,
    IFormFile FileStream);
