using FoodRush.Application.Abstractions.Messaging;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Domain.Restaurants.DomainEvents.Document;
using Microsoft.Extensions.Logging;

namespace FoodRush.Application.Features.Restaurants.DomainEvents;

internal class DocumentReplacedDomainEventHandler
    (IDocumentStorageService storageService,
    ILogger<DocumentReplacedDomainEventHandler> logger) :
    IDomainEventHandler<RestaurantDocumentFileReplacedDomainEvent>
{
    public async Task Handle(RestaurantDocumentFileReplacedDomainEvent notification, CancellationToken cancellationToken)
    {
        var deleteResult = await storageService.DeleteAsync(notification.OldDocumentPublicId.Value, cancellationToken);

        if (deleteResult.IsFailure)
        {
            logger.LogError(
                "Failed to delete old document with public ID {PublicId}. Error: {Error}",
                notification.OldDocumentPublicId.Value,
                deleteResult.Error);

            return;
        }

        logger.LogInformation(
            "Old document with public ID {PublicId} deleted successfully.",
            notification.OldDocumentPublicId.Value);
    }
}
