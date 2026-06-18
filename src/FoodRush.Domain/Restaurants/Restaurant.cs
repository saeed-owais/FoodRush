using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Interfaces;
using FoodRush.Domain.Restaurants.DomainEvents;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.Enums;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants;

public sealed class Restaurant : AggregateRoot<RestaurantId>, IAuditable, ISoftDeletable
{
    private Restaurant() { }
    private Restaurant(
        RestaurantId id,
        UserId ownerId,
        Name name,
        RestaurantStatus status,
        AverageRating averageRating,
        Latitude latitude,
        Longitude longitude,
        DeliveryRadiusKm deliveryRadius)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Status = status;
        AverageRating = averageRating;
        Latitude = latitude;
        Longitude = longitude;
        DeliveryRadiusKm = deliveryRadius;
    }

    #region Props
    private readonly List<RestaurantDocument> _documents = new();
    public IReadOnlyCollection<RestaurantDocument> Documents => _documents.AsReadOnly();

    public UserId OwnerId { get; private set; }
    public Name Name { get; private set; }
    public RestaurantStatus Status { get; private set; }
    public AverageRating AverageRating { get; private set; }
    public Latitude Latitude { get; private set; }
    public Longitude Longitude { get; private set; }
    public DeliveryRadiusKm DeliveryRadiusKm { get; private set; }
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
            RestaurantStatus.PendingDocuments,
            AverageRating.Zero(),
            latitude,
            longitude,
            deliveryRadius);

        restaurant.Raise(new RestaurantRegisteredDomainEvent(Guid.NewGuid(), restaurant.Id));

        return restaurant;
    }

    public Result SubmitDocument(RestaurantDocument document)
    {
        if (document is null)
            return Result.Failure(
                RestaurantErrors.DocumentCannotBeNull);

        if (Status != RestaurantStatus.PendingDocuments)
            return Result.Failure(
                RestaurantErrors.DocumentsCanOnlyBeSubmittedWhilePending);

        var existingDocument =
            _documents.FirstOrDefault(
                d => d.Type == document.Type);

        if (existingDocument != null)
        {
            var updateResult =
                existingDocument.Resubmit(
                    document.FileUrl);

            if (updateResult.IsFailure)
            {
                return updateResult;
            }
        }
        else
        {
            _documents.Add(document);
        }

        if (HasAllRequiredDocuments())
        {
            Status = RestaurantStatus.DocumentsSubmitted;
        }

        return Result.Success();
    }

    public Result SubmitDocuments(IEnumerable<RestaurantDocument> documents)
    {
        if (documents is null)
            return Result.Failure(
                RestaurantErrors.DocumentsCollectionCannotBeEmpty);

        var documentsList = documents.ToList();

        if (documentsList.Count == 0)
            return Result.Failure(
                RestaurantErrors.DocumentsCollectionCannotBeEmpty);

        if (Status != RestaurantStatus.PendingDocuments)
            return Result.Failure(
                RestaurantErrors.DocumentsCanOnlyBeSubmittedWhilePending);

        if (documentsList
            .GroupBy(d => d.Type)
            .Any(g => g.Count() > 1))
        {
            return Result.Failure(
                RestaurantErrors.DuplicateDocumentTypes);
        }

        foreach (var document in documentsList)
        {
            var existingDocument =
                _documents.FirstOrDefault(
                    d => d.Type == document.Type);

            if (existingDocument != null)
            {
                var updateResult =
                    existingDocument.Resubmit(
                        document.FileUrl);

                if (updateResult.IsFailure)
                {
                    return updateResult;
                }
            }
            else
            {
                _documents.Add(document);
            }
        }

        if (HasAllRequiredDocuments())
        {
            Status = RestaurantStatus.DocumentsSubmitted;
        }

        return Result.Success();
    }

    public Result Approve()
    {
        if (Status != RestaurantStatus.DocumentsSubmitted)
            return Result.Failure(
                RestaurantErrors.RestaurantMustBeSubmittedBeforeApproval);

        if (!HasAllRequiredDocuments())
            return Result.Failure(
                RestaurantErrors.AllRequiredDocumentsMustBeUploaded);

        if (!AllDocumentsApproved())
        {
            return Result.Failure(
                RestaurantErrors.AllDocumentsMustBeApprovedForRestaurantApproval);
        }

        Status = RestaurantStatus.Approved;

        Raise(new RestaurantApprovedDomainEvent(Guid.NewGuid(), Id));

        return Result.Success();
    }

    public Result Reject()
    {
        if (Status != RestaurantStatus.DocumentsSubmitted)
            return Result.Failure(RestaurantErrors.RestaurantMustBeSubmittedBeforeRejection);

        Status = RestaurantStatus.Rejected;

        Raise(new RestaurantRejectedDomainEvent(Guid.NewGuid(), Id));

        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status != RestaurantStatus.Approved)
            return Result.Failure(RestaurantErrors.OnlyApprovedRestaurantsCanBeSuspended);

        Status = RestaurantStatus.Suspended;

        Raise(new RestaurantSuspendedDomainEvent(Guid.NewGuid(), Id));

        return Result.Success();
    }

    private bool HasAllRequiredDocuments()
    {
        return _documents.Any(d => d.Type == DocumentType.BankAccountProof) &&
               _documents.Any(d => d.Type == DocumentType.CommercialRegistration) &&
               _documents.Any(d => d.Type == DocumentType.FoodLicense) &&
               _documents.Any(d => d.Type == DocumentType.HealthCertificate) &&
               _documents.Any(d => d.Type == DocumentType.NationalId) &&
               _documents.Any(d => d.Type == DocumentType.OwnershipContract) &&
               _documents.Any(d => d.Type == DocumentType.TaxCard);
    }

    private bool AllDocumentsApproved()
    {
        return _documents.All(d => d.Status == DocumentStatus.Approved);
    }

}
