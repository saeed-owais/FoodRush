using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Restaurants.UploadDocument;

internal sealed class UploadDocumentCommandHandler
    (IDocumentStorageService storageService,
    IRestaurantRepository restaurantRepository,
    IUserContext userContext,
    ILogger<UploadDocumentCommandHandler> logger) : ICommandHandler<UploadDocumentCommand, RestaurantDocument>
{
    public async Task<Result<RestaurantDocument>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = new UserId(userContext.UserId);

        var restaurantId = new RestaurantId(request.RestaurantId);

        var restaurant = await restaurantRepository
            .GetWithDocumentsAsync(
                restaurantId,
                cancellationToken);

        if (restaurant == null)
            return Result.Failure<RestaurantDocument>(
                RestaurantErrors.NotFound(restaurantId));

        if (restaurant.OwnerId != currentUserId)
        {
            return Result.Failure<RestaurantDocument>(
                RestaurantErrors.NotRestaurantOwner);
        }

        var uploadResult = await storageService.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            request.FileSize,
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<RestaurantDocument>(
                uploadResult.Error);
        }

        Result<FileUrl> fileUrlResult = FileUrl.Create(uploadResult.Value.DownloadUrl);

        if (fileUrlResult.IsFailure)
        {
            logger.LogWarning(
                "Failed to create FileUrl for restaurant {RestaurantId}: {Error}",
                restaurantId,
                fileUrlResult.Error);

            await storageService.DeleteAsync(uploadResult.Value.PublicId, cancellationToken);

            return Result.Failure<RestaurantDocument>(
                fileUrlResult.Error);
        }

        var documentResult = restaurant.UploadDocument(request.DocumentType, fileUrlResult.Value);

        if (documentResult.IsFailure)
        {
            logger.LogWarning(
                "Failed to upload document for restaurant {RestaurantId}: {Error}",
                restaurantId,
                documentResult.Error);

            var deleteResult = await storageService.DeleteAsync(uploadResult.Value.PublicId, cancellationToken);

            if (deleteResult.IsFailure)
            {
                logger.LogError(
                    "Failed to delete uploaded document for restaurant {RestaurantId}: {Error}",
                    restaurantId,
                    deleteResult.Error);
            }

            return documentResult;
        }

        return documentResult;
    }

}
