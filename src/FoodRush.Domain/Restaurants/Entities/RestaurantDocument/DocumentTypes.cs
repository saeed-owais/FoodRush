namespace FoodRush.Domain.Restaurants.Entities.RestaurantDocument;

public static class DocumentTypes
{
    public static readonly IReadOnlySet<DocumentType> Required =
        new HashSet<DocumentType>
        {
            DocumentType.BankAccountProof,
            DocumentType.CommercialRegistration,
            DocumentType.FoodLicense,
            DocumentType.HealthCertificate,
            DocumentType.NationalId,
            DocumentType.OwnershipContract,
            DocumentType.TaxCard
        };
}