using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Restaurants.ResubmitDocument;

internal sealed class ResubmitDocumentCommandHandler
    (IRestaurantRepository restaurantRepository,
    IDocumentStorageService documentStorageService,
    ILogger<ResubmitDocumentCommandHandler> logger)
    : ICommandHandler<ResubmitDocumentCommand>
{
    public async Task<Result> Handle(ResubmitDocumentCommand request, CancellationToken cancellationToken)
    {
        var restaurantId = new RestaurantId(request.RestaurantId);
        var documentId = new DocumentId(request.DocumentId);

        var restaurant = await restaurantRepository.GetWithDocumentByIdAsync(
           restaurantId,
           documentId,
           cancellationToken);

        if (restaurant is null)
        {
            logger.LogWarning(
                "Restaurant {RestaurantId} was not found while resubmitting document {DocumentId}.",
                restaurantId,
                documentId);
            return Result.Failure(RestaurantErrors.NotFound(restaurantId));
        }

        var canResubmitResult = restaurant.EnsureDocumentCanBeResubmitted(documentId);

        if (canResubmitResult.IsFailure)
        {
            logger.LogWarning(
                "Document {DocumentId} for restaurant {RestaurantId} cannot be resubmitted. Error: {Error}",
                documentId,
                restaurantId,
                canResubmitResult.Error);
            return canResubmitResult;
        }

        var uploadResult = await documentStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            request.FileSize,
            cancellationToken);

        if (uploadResult.IsFailure)
        {
            logger.LogError(
                "Failed to upload document {DocumentId} for restaurant {RestaurantId}. Error: {Error}",
                documentId,
                restaurantId,
                uploadResult.Error);

            return Result.Failure(uploadResult.Error);
        }

        var fileUrlResult = FileUrl.Create(uploadResult.Value.DownloadUrl);

        if (fileUrlResult.IsFailure)
        {
            await CleanupUploadedFileAsync(
                uploadResult.Value.PublicId,
                documentId,
                restaurantId,
                cancellationToken);

            logger.LogWarning(
                "Failed to create file URL for document {DocumentId} for restaurant {RestaurantId}.",
                documentId,
                restaurantId);
            return Result.Failure(fileUrlResult.Error);
        }

        var publicIdResult = PublicId.Create(uploadResult.Value.PublicId);

        if (publicIdResult.IsFailure)
        {
            await CleanupUploadedFileAsync(
                uploadResult.Value.PublicId,
                documentId,
                restaurantId,
                cancellationToken);

            logger.LogError(
                "Failed to create public ID for document {DocumentId} for restaurant {RestaurantId}. Error: {Error}",
                documentId,
                restaurantId,
                publicIdResult.Error);
            return Result.Failure(publicIdResult.Error);
        }

        var fileUrl = fileUrlResult.Value;
        var publicId = publicIdResult.Value;

        var updatedRestaurantResult = restaurant.ResubmitDocument(
            documentId,
            fileUrl,
            publicId);

        if (updatedRestaurantResult.IsFailure)
        {
            await CleanupUploadedFileAsync(
                uploadResult.Value.PublicId,
                documentId,
                restaurantId,
                cancellationToken);

            logger.LogError(
                "Failed to resubmit document {DocumentId} for restaurant {RestaurantId}. Error: {Error}",
                documentId,
                restaurantId,
                updatedRestaurantResult.Error);

            return Result.Failure(updatedRestaurantResult.Error);
        }

        return updatedRestaurantResult;
    }

    private async Task CleanupUploadedFileAsync(
        string publicId,
        DocumentId documentId,
        RestaurantId restaurantId,
        CancellationToken cancellationToken)
    {
        var result = await documentStorageService.DeleteAsync(publicId, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError(
                "Failed to delete uploaded document {DocumentId} for restaurant {RestaurantId}. Error: {Error}",
                documentId,
                restaurantId,
                result.Error);
        }
    }
}
