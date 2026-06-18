namespace FoodRush.Domain.Common.Errors;

internal static class RestaurantErrors
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

    public static readonly Error DocumentsCanOnlyBeSubmittedWhilePending =
        Error.Validation(
            "Restaurant.Documents.CanOnlyBeSubmittedWhilePending",
            "Documents can only be submitted while the restaurant is in the PendingDocuments state.");

    public static readonly Error RestaurantMustBeSubmittedBeforeApproval =
    Error.Validation(
        "Restaurant.Approval.RequiresSubmittedDocuments",
        "The restaurant must be in the DocumentsSubmitted state before it can be approved.");

    public static readonly Error RestaurantMustBeSubmittedBeforeRejection =
        Error.Validation(
            "Restaurant.Rejection.RequiresSubmittedDocuments",
            "The restaurant must be in the DocumentsSubmitted state before it can be rejected.");

    public static readonly Error OnlyApprovedRestaurantsCanBeSuspended =
        Error.Validation(
            "Restaurant.Suspension.OnlyApproved",
            "Only approved restaurants can be suspended.");

    public static readonly Error InvalidDocumentUrl =
        Error.Validation(
            "Restaurant.Document.InvalidUrl",
            "Document URL cannot be null or empty.");

    public static readonly Error DocumentsCollectionCannotBeEmpty =
        Error.Validation(
            "Restaurant.Documents.EmptyCollection",
            "At least one document must be provided when submitting documents.");

    public static readonly Error DocumentCannotBeNull =
        Error.Validation("Restaurant.Document.Null", "The specified document cannot be null.");

    public static readonly Error AllRequiredDocumentsMustBeUploaded =
        Error.Validation(
            "Restaurant.Documents.AllRequiredMustBeUploaded",
            "All required documents must be uploaded before the restaurant can be approved.");

    public static readonly Error AllDocumentsMustBeApprovedForRestaurantApproval =
        Error.Validation(
            "Restaurant.Approval.AllDocumentsMustBeApproved",
            "All documents must be approved before the restaurant can be approved.");

    public static readonly Error DuplicateDocumentTypes =
        Error.Validation(
            "Restaurant.Documents.DuplicateTypes",
            "Duplicate document types are not allowed in the documents collection.");

    public static readonly Error CannotResubmitApprovedDocument =
        Error.Validation(
            "Restaurant.Document.Approved.CannotResubmit",
            "An approved document cannot be resubmitted.");


}
