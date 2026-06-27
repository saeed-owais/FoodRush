using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

namespace FoodRush.Application.Features.Restaurants.UploadDocument;

public sealed record UploadDocumentCommand(
    Guid RestaurantId,
    DocumentType DocumentType,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize) : ICommand<RestaurantDocument>;