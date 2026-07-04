using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Restaurants.ResubmitDocument;

public sealed record ResubmitDocumentCommand(
    Guid RestaurantId,
    Guid DocumentId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize) : ICommand;