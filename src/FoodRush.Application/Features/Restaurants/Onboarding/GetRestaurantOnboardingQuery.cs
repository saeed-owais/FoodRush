using FoodRush.Application.Abstractions.Messaging;

namespace FoodRush.Application.Features.Restaurants.Onboarding;

public sealed record GetRestaurantOnboardingQuery(Guid RestaurantId)
    : IQuery<RestaurantOnboardingResponse>;