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
       FileUrl fileUrl,
       PublicId publicId)
    {
        Id = new DocumentId(Guid.CreateVersion7());
        RestaurantId = restaurantId;
        Type = type;
        FileUrl = fileUrl;
        PublicId = publicId;
        Status = DocumentStatus.Draft;
    }

    public RestaurantId RestaurantId { get; private set; }
    public FileUrl FileUrl { get; private set; }
    public PublicId PublicId { get; private set; }
    public RejectionReason? RejectionReason { get; private set; }
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
    public Result Reject(string reason)
    {
        if (Status != DocumentStatus.UnderReview)
        {
            return Result.Failure(RestaurantErrors.DocumentMustBeUnderReview);
        }

        Status = DocumentStatus.Rejected;

        var rejectionReason = RejectionReason.Create(reason);

        if (rejectionReason.IsFailure)
        {
            return rejectionReason;
        }

        RejectionReason = rejectionReason.Value;
        Status = DocumentStatus.Rejected;

        return Result.Success();
    }

    internal Result Replace(FileUrl fileUrl, PublicId publicId)
    {
        if (Status != DocumentStatus.Draft)
        {
            return Result.Failure(
                RestaurantErrors.DocumentCanOnlyBeReplacedWhileDraft);
        }

        FileUrl = fileUrl;
        PublicId = publicId;

        return Result.Success();
    }

    internal Result Resubmit(FileUrl fileUrl, PublicId publicId)
    {
        if (Status != DocumentStatus.Rejected)
        {
            return Result.Failure(
                RestaurantErrors.OnlyRejectedDocumentsCanBeResubmitted);
        }

        FileUrl = fileUrl;
        PublicId = publicId;

        Status = DocumentStatus.Draft;

        return Result.Success();
    }
    internal Result CanResubmit()
    {
        if (Status != DocumentStatus.Rejected)
        {
            return Result.Failure(
                RestaurantErrors.OnlyRejectedDocumentsCanBeResubmitted);
        }

        return Result.Success();
    }

}