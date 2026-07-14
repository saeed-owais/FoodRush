using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Interfaces;
using FoodRush.Domain.Restaurants.DomainEvents;
using FoodRush.Domain.Restaurants.DomainEvents.Document;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.Enums;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants;

public sealed class Restaurant : AggregateRoot<RestaurantId>, IAuditable, ISoftDeletable
{
    private readonly List<RestaurantDocument> _documents = new();

    private Restaurant() { }
    private Restaurant(
        RestaurantId id,
        UserId ownerId,
        Name name,
        Latitude latitude,
        Longitude longitude,
        DeliveryRadiusKm deliveryRadius)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        DeliveryRadiusKm = deliveryRadius;

        Status = RestaurantStatus.Draft;
        AverageRating = AverageRating.Zero();
    }

    #region Props
    public IReadOnlyCollection<RestaurantDocument> Documents => _documents.AsReadOnly();

    public UserId OwnerId { get; private set; }
    public Name Name { get; private set; }
    public RestaurantStatus Status { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public Latitude Latitude { get; private set; }
    public Longitude Longitude { get; private set; }
    public DeliveryRadiusKm DeliveryRadiusKm { get; private set; }
    public DateTime? SuspendedAt { get; private set; }
    public string? SuspensionReason { get; private set; }
    public bool IsVisible => Status == RestaurantStatus.Approved;

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    #endregion

    public static Result<Restaurant> Register(
        UserId ownerId,
        Name name,
        Latitude latitude,
        Longitude longitude,
        DeliveryRadiusKm deliveryRadius)
    {
        var restaurant = new Restaurant(
            new RestaurantId(Guid.CreateVersion7()),
            ownerId,
            name,
            latitude,
            longitude,
            deliveryRadius);

        restaurant.Raise(new RestaurantRegisteredDomainEvent(restaurant.Id));

        return restaurant;
    }

    public Result<RestaurantDocument> UploadDocument(DocumentType documentType, FileUrl fileUrl, PublicId publicId)
    {
        if (Status != RestaurantStatus.Draft)
        {
            return Result.Failure<RestaurantDocument>(
                RestaurantErrors.DocumentsCanOnlyBeUploadedInDraftState);
        }

        var existingDocument = _documents.FirstOrDefault(d => d.Type == documentType);

        if (existingDocument is not null)
        {
            var oldPublicId = existingDocument.PublicId;

            var result = existingDocument.Replace(fileUrl, publicId);

            if (result.IsFailure)
            {
                return Result.Failure<RestaurantDocument>(result.Error);
            }

            Raise(new RestaurantDocumentFileReplacedDomainEvent(
                Id,
                existingDocument.Id,
                oldPublicId,
                publicId));

            return Result.Success(existingDocument);
        }

        var document = new RestaurantDocument(Id, documentType, fileUrl, publicId);

        _documents.Add(document);

        Raise(new RestaurantDocumentUploadedDomainEvent(
            Id,
            document.Id,
            document.Type));

        return document;
    }

    public Result SubmitForReview()
    {
        if (Status != RestaurantStatus.Draft)
        {
            return Result.Failure(
                RestaurantErrors.RestaurantMustBeInDraftStateBeforeSubmission);
        }

        if (!HasAllRequiredDocuments())
        {
            return Result.Failure(
                RestaurantErrors.AllRequiredDocumentsMustBeUploaded);
        }

        if (HasRejectedDocuments())
        {
            return Result.Failure(
                RestaurantErrors.RejectedDocumentsMustBeResubmitted);
        }

        Status = RestaurantStatus.UnderReview;

        foreach (var document in _documents)
        {
            if (document.Status == DocumentStatus.Draft)
                document.MarkAsUnderReview();
        }

        Raise(new RestaurantSubmittedForReviewDomainEvent(Id));

        return Result.Success();
    }

    public Result ApproveDocument(DocumentId documentId)
    {
        if (Status != RestaurantStatus.UnderReview)
        {
            return Result.Failure(
                RestaurantErrors.RestaurantMustBeUnderReviewBeforeApproval);
        }

        var document = GetDocument(documentId);

        if (document == null)
        {
            return Result.Failure(
                RestaurantErrors.DocumentNotFound);
        }

        var result = document.Approve();

        if (result.IsFailure)
        {
            return result;
        }

        Raise(new RestaurantDocumentApprovedDomainEvent(Id, document.Id, Name.Value));

        TryApproveRestaurant();

        return result;
    }

    public Result Suspend(string reason)
    {
        if (Status != RestaurantStatus.Approved)
            return Result.Failure(RestaurantErrors.OnlyApprovedRestaurantsCanBeSuspended);

        Status = RestaurantStatus.Suspended;

        SuspendedAt = DateTime.UtcNow;
        SuspensionReason = reason;

        Raise(new RestaurantSuspendedDomainEvent(Id));

        return Result.Success();
    }

    public Result RejectDocument(DocumentId documentId, string reason)
    {
        if (Status != RestaurantStatus.UnderReview)
        {
            return Result.Failure(
                RestaurantErrors.RestaurantMustBeUnderReviewBeforeRejection);
        }

        var document = GetDocument(documentId);

        if (document == null)
        {
            return Result.Failure(
                RestaurantErrors.DocumentNotFound);
        }

        var result = document.Reject(reason);

        if (result.IsFailure)
        {
            return result;
        }

        Status = RestaurantStatus.Draft;

        Raise(new RestaurantDocumentRejectedDomainEvent(Id, document.Id));

        return Result.Success();
    }

    public Result EnsureDocumentCanBeResubmitted(DocumentId documentId)
    {
        if (Status != RestaurantStatus.Draft)
        {
            return Result.Failure(RestaurantErrors.RestaurantMustBeInDraftState);
        }

        var document = GetDocument(documentId);

        if (document is null)
        {
            return Result.Failure(RestaurantErrors.DocumentNotFound);
        }

        return document.CanResubmit();
    }
    public Result ResubmitDocument(DocumentId documentId, FileUrl fileUrl, PublicId publicId)
    {
        if (Status != RestaurantStatus.Draft)
        {
            return Result.Failure(
                RestaurantErrors.RestaurantMustBeInDraftState);
        }

        var document = GetDocument(documentId);

        if (document == null)
        {
            return Result.Failure(
                RestaurantErrors.DocumentNotFound);
        }

        var oldPublicId = document.PublicId;

        var result = document.Resubmit(fileUrl, publicId);

        if (result.IsSuccess)
        {
            Raise(new RestaurantDocumentFileReplacedDomainEvent(
                Id,
                document.Id,
                oldPublicId,
                publicId));
        }

        return result;
    }

    public Result Reactivate()
    {
        if (Status != RestaurantStatus.Suspended)
        {
            return Result.Failure(
                RestaurantErrors.OnlySuspendedRestaurantsCanBeReactivated);
        }

        Status = RestaurantStatus.Approved;

        SuspendedAt = null;
        SuspensionReason = null;

        Raise(new RestaurantReactivatedDomainEvent(Id));

        return Result.Success();
    }

    public Result UpdateOperationalInformation(Name name, DeliveryRadiusKm deliveryRadiusKm)
    {
        if (Status != RestaurantStatus.Approved)
        {
            return Result.Failure(
                RestaurantErrors.OnlyApprovedRestaurantsCanBeUpdated);
        }

        Name = name;
        DeliveryRadiusKm = deliveryRadiusKm;

        return Result.Success();
    }

    private bool HasAllRequiredDocuments()
    {
        return DocumentTypes.Required.All(requiredType => _documents.Any(doc => doc.Type == requiredType));
    }

    private RestaurantDocument? GetDocument(DocumentId documentId)
    {
        return _documents.FirstOrDefault(x => x.Id == documentId);
    }

    private void TryApproveRestaurant()
    {
        if (Status != RestaurantStatus.UnderReview)
        {
            return;
        }

        if (!AllRequiredDocumentsApproved())
        {
            return;
        }

        Status = RestaurantStatus.Approved;

        Raise(new RestaurantApprovedDomainEvent(Id));
    }

    private bool AllRequiredDocumentsApproved()
    {
        return DocumentTypes.Required.All(
        requiredType =>
            _documents.Any(
                d => d.Type == requiredType &&
                     d.Status == DocumentStatus.Approved));
    }

    private bool HasRejectedDocuments()
    {
        return _documents.Any(
            d => d.Status == DocumentStatus.Rejected);
    }
}
