using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Restaurants;
using FoodRush.Domain.Restaurants.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Infrastructure.Persistence.Repositories;

internal sealed class RestaurantRepository(IApplicationDbContext context) : IRestaurantRepository
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
}
