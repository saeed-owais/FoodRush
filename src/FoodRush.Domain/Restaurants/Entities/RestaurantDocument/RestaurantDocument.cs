using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

public sealed class RestaurantDocument : Entity<DocumentId>
{
    private RestaurantDocument()
    {
    }
    internal RestaurantDocument(
       RestaurantId restaurantId,
       DocumentType type,
       FileUrl fileUrl)
    {
        Id = new DocumentId(Guid.CreateVersion7());
        RestaurantId = restaurantId;
        Type = type;
        FileUrl = fileUrl;
        Status = DocumentStatus.Draft;
    }

    public RestaurantId RestaurantId { get; private set; }
    public FileUrl FileUrl { get; private set; }
    public DocumentType Type { get; private set; }

    public DocumentStatus Status { get; private set; }

    internal Result Approve()
    {
        if (Status != DocumentStatus.UnderReview)
        {
            return Result.Failure(
                RestaurantErrors.DocumentMustBeUnderReview);
        }

        Status = DocumentStatus.Approved;

        return Result.Success();
    }

    internal void MarkAsUnderReview()
    {
        Status = DocumentStatus.UnderReview;
    }

    internal void Reject()
    {
        Status = DocumentStatus.Rejected;
    }

    internal Result Replace(FileUrl fileUrl)
    {
        if (Status != DocumentStatus.Draft)
        {
            return Result.Failure(
                RestaurantErrors.DocumentCanOnlyBeReplacedWhileDraft);
        }

        FileUrl = fileUrl;

        return Result.Success();
    }

    internal Result Resubmit(FileUrl fileUrl)
    {
        if (Status != DocumentStatus.Rejected)
        {
            return Result.Failure(
                RestaurantErrors.OnlyRejectedDocumentsCanBeResubmitted);
        }

        FileUrl = fileUrl;

        Status = DocumentStatus.Draft;

        return Result.Success();
    }

}