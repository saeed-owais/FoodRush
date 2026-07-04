using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;

public sealed record GetRestaurantDetailsForReviewQuery(Guid RestaurantId)
    : IQuery<RestaurantDetailsForReviewResponse>;