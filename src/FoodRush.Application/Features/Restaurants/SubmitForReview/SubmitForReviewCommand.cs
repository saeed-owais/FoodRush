
using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Restaurants.SubmitForReview;

public sealed record SubmitForReviewCommand(Guid RestaurantId) : ICommand;