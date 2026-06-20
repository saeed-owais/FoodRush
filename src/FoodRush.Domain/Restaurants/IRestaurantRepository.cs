using FoodRush.Domain.Entities.Identity;
using FoodRush.Domain.Restaurants.ValueObjects;

namespace FoodRush.Domain.Restaurants;

public interface IRestaurantRepository
{
    void Add(Restaurant restaurant);

    Task<Restaurant?> GetByIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken);

    Task<Restaurant?> GetWithDocumentsAsync(RestaurantId restaurantId, CancellationToken cancellationToken);

    Task<Restaurant?> GetByOwnerIdAsync(UserId ownerId, CancellationToken cancellationToken);

}
