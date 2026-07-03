using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.RejectRestaurantDocument;

public sealed record RejectRestaurantDocumentCommand(
    Guid RestaurantId,
    Guid DocumentId,
    string Reason)
    : ICommand;