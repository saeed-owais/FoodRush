using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

public sealed class RestaurantDocument : Entity<DocumentId>
{
    private RestaurantDocument(FileUrl fileUrl)
    {
        FileUrl = fileUrl;
    }
    private RestaurantDocument()
    {
    }
    public RestaurantId RestaurantId { get; private set; }
    public FileUrl FileUrl { get; private set; }
    public DocumentType Type { get; private set; }

    public DocumentStatus Status { get; private set; }

    public static RestaurantDocument Create(FileUrl fileUrl, DocumentType documentType, RestaurantId restaurantId)
    {
        return new RestaurantDocument(fileUrl)
        {
            Id = new DocumentId(Guid.CreateVersion7()),
            Type = documentType,
            RestaurantId = restaurantId,
            Status = DocumentStatus.Pending
        };
    }

    public void Approve()
    {
        Status = DocumentStatus.Approved;
    }

    public void Reject()
    {
        Status = DocumentStatus.Rejected;
    }

    public Result Resubmit(FileUrl fileUrl)
    {
        if (Status == DocumentStatus.Approved)
        {
            return Result.Failure(
                RestaurantErrors.CannotResubmitApprovedDocument);
        }

        FileUrl = fileUrl;

        Status = DocumentStatus.Pending;

        return Result.Success();
    }
}