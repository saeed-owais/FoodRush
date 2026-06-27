using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Restaurants.RegisterRestaurant;

internal sealed class RegisterRestaurantCommandHandler
    (IUserContext userContext,
    IRestaurantRepository repository,
    ILogger<RegisterRestaurantCommandHandler> logger)
    : ICommandHandler<RegisterRestaurantCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterRestaurantCommand request, CancellationToken cancellationToken)
    {
        var ownerId = new UserId(userContext.UserId);

        var ownerExists = await repository.GetByOwnerIdAsync(ownerId, cancellationToken);

        if (ownerExists != null)
        {
            logger.LogWarning("User {UserId} already owns a restaurant", userContext.UserId);
            return Result.Failure<Guid>(
                Error.Conflict("User.OwnsRestaurant", "User already owns a restaurant"));
        }

        var nameResult = Name.Create(request.Name);
        if (nameResult.IsFailure)
        {
            logger.LogWarning("Failed to create restaurant name: {Error}", nameResult.Error);
            return Result.Failure<Guid>(nameResult.Error);
        }

        var latitudeResult = Latitude.Create(request.Latitude);
        if (latitudeResult.IsFailure)
        {
            logger.LogWarning("Failed to create restaurant latitude: {Error}", latitudeResult.Error);
            return Result.Failure<Guid>(latitudeResult.Error);
        }

        var longitudeResult = Longitude.Create(request.Longitude);
        if (longitudeResult.IsFailure)
        {
            logger.LogWarning("Failed to create restaurant longitude: {Error}", longitudeResult.Error);
            return Result.Failure<Guid>(longitudeResult.Error);
        }

        var deliveryRadiusKmResult = DeliveryRadiusKm.Create(request.DeliveryRadiusKm);
        if (deliveryRadiusKmResult.IsFailure)
        {
            logger.LogWarning("Failed to create restaurant delivery radius: {Error}", deliveryRadiusKmResult.Error);
            return Result.Failure<Guid>(deliveryRadiusKmResult.Error);
        }

        var restaurantResult = Restaurant.Register(
            ownerId,
            nameResult.Value,
            latitudeResult.Value,
            longitudeResult.Value,
            deliveryRadiusKmResult.Value);

        if (restaurantResult.IsFailure)
        {
            logger.LogWarning("Failed to register restaurant: {Error}", restaurantResult.Error);
            return Result.Failure<Guid>(restaurantResult.Error);
        }

        repository.Add(restaurantResult.Value);

        return restaurantResult.Value.Id.Value;
    }
}
