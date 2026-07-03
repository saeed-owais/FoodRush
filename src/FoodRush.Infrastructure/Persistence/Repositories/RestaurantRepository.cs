using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.Entities.RestaurantDocument;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Repositories;

internal sealed class RestaurantRepository(ApplicationDbContext context) : IRestaurantRepository
{
    public void Add(Restaurant restaurant)
    {
        context.Restaurants.Add(restaurant);
    }

    public async Task<Restaurant?> GetByIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        return await context.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId, cancellationToken);
    }

    public async Task<Restaurant?> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken)
    {
        return await context.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == ownerId, cancellationToken);
    }

    public async Task<Restaurant?> GetWithDocumentByIdAsync(RestaurantId restaurantId, DocumentId documentId, CancellationToken cancellationToken)
    {
        return await context.Restaurants
            .AsTracking()
            .Include(r => r.Documents.Where(d => d.Id == documentId))
            .FirstOrDefaultAsync(
                r => r.Id == restaurantId,
                cancellationToken);
    }

    public async Task<Restaurant?> GetWithDocumentsAsync(RestaurantId restaurantId, CancellationToken cancellationToken)
    {
        return await context.Restaurants
            .AsTracking()
            .Include(r => r.Documents)
            .FirstOrDefaultAsync(r => r.Id == restaurantId, cancellationToken);
    }
}
