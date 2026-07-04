using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.ApproveRestaurantDocument;

public sealed record ApproveRestaurantDocumentCommand(
    Guid RestaurantId,
    Guid DocumentId)
    : ICommand;