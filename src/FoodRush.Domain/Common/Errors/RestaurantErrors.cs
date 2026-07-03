using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Common.Errors;

public static class RestaurantErrors
{
    public static readonly Error NameIsRequired =
        Error.Validation("Restaurant.Name.Required", "Restaurant name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Restaurant.Name.TooLong", "Restaurant name cannot be longer than 100 characters.");

    public static readonly Error DeliveryRadiusCannotBeNegative =
        Error.Validation("Restaurant.DeliveryRadiusKm.Negative", "Delivery radius cannot be negative.");

    public static readonly Error InvalidLongitude =
        Error.Validation("Restaurant.Longitude.Invalid", "Longitude must be between -180 and 180.");

    public static readonly Error InvalidLatitude =
        Error.Validation("Restaurant.Latitude.Invalid", "Latitude must be between -90 and 90.");

    public static readonly Error DocumentsCanOnlyBeUploadedInDraftState =
        Error.Validation(
            "Restaurant.Documents.CanOnlyBeUploadedInDraftState",
            "Documents can only be uploaded while the restaurant is in the Draft state.");

    public static readonly Error RestaurantMustBeUnderReviewBeforeApproval =
    Error.Validation(
        "Restaurant.Approval.RequiresUnderReview",
        "The restaurant must be in the UnderReview state before it can be approved.");

    public static readonly Error RestaurantMustBeUnderReviewBeforeRejection =
        Error.Validation(
            "Restaurant.Rejection.RequiresUnderReview",
            "The restaurant must be in the UnderReview state before it can be rejected.");

    public static readonly Error OnlyApprovedRestaurantsCanBeSuspended =
        Error.Validation(
            "Restaurant.Suspension.OnlyApproved",
            "Only approved restaurants can be suspended.");

    public static readonly Error InvalidDocumentUrl =
        Error.Validation(
            "Restaurant.Document.InvalidUrl",
            "Document URL cannot be null or empty.");

    public static readonly Error DocumentCannotBeNull =
        Error.Validation("Restaurant.Document.Null", "The specified document cannot be null.");

    public static readonly Error AllRequiredDocumentsMustBeUploaded =
        Error.Validation(
            "Restaurant.Documents.AllRequiredMustBeUploaded",
            "All required documents must be uploaded before the restaurant can be approved.");

    public static readonly Error OnlyRejectedDocumentsCanBeResubmitted =
        Error.Validation(
            "Restaurant.Document.Rejected.OnlyRejectedCanBeResubmitted",
            "Only rejected documents can be resubmitted.");

    public static readonly Error DocumentNotFound =
        Error.NotFound("Restaurant.Document.NotFound", "The specified document was not found.");

    public static readonly Error DocumentMustBeUnderReview =
        Error.Validation(
            "Restaurant.Document.MustBeUnderReview",
            "The specified document must be in an under-review state.");

    public static readonly Error OnlySuspendedRestaurantsCanBeReactivated =
        Error.Validation(
            "Restaurant.Reactivation.OnlySuspended",
            "Only suspended restaurants can be reactivated.");

    public static readonly Error OnlyApprovedRestaurantsCanBeUpdated =
        Error.Validation(
            "Restaurant.Update.OnlyApproved",
            "Only approved restaurants can be updated.");

    public static readonly Error DocumentCanOnlyBeReplacedWhileDraft =
        Error.Validation(
            "Restaurant.Document.CanOnlyBeReplacedWhileDraft",
            "The specified document can only be replaced while it is in the Draft state.");

    public static readonly Error RejectedDocumentsMustBeResubmitted =
        Error.Validation(
            "Restaurant.Documents.Rejected.MustBeResubmitted",
            "All rejected documents must be resubmitted before the restaurant can be approved.");

    public static readonly Error RestaurantMustBeInDraftState =
        Error.Validation(
            "Restaurant.State.Draft.Required",
            "The restaurant must be in the Draft state to perform this action.");

    public static readonly Error RestaurantMustBeInDraftStateBeforeSubmission =
        Error.Validation(
            "Restaurant.Submission.RequiresDraft",
            "The restaurant must be in the Draft state before it can be submitted for review.");

    public static readonly Error DocumentCanOnlyBeMarkedUnderReviewWhileDraft =
        Error.Validation(
            "Restaurant.Document.CanOnlyBeMarkedUnderReviewWhileDraft",
            "The specified document can only be marked as under review while it is in the Draft state.");

    public static readonly Error NotRestaurantOwner =
        Error.Validation(
            "Restaurant.Owner.Invalid",
            "The specified user is not the owner of the restaurant.");

    public static readonly Error InvalidDocumentPublicId =
        Error.Validation(
            "Restaurant.Document.InvalidPublicId",
            "The specified document public ID is invalid.");

    public static readonly Error InvalidDocumentRejectionReason =
        Error.Validation(
            "Restaurant.Document.InvalidRejectionReason",
            "The specified document rejection reason is invalid.");

    public static Error NotFound(RestaurantId restaurantId)
    {
        return Error.NotFound(
            "Restaurant.NotFound",
            $"Restaurant with id {restaurantId.Value} not found.");
    }
}
