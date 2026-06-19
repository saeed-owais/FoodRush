using FoodRush.API.Extensions;
using FoodRush.Application.Features.Restaurants.RegisterRestaurant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        //[HasPermission("register:restaurant")]
        public async Task<IActionResult> RegisterRestaurant(RegisterRestaurantCommand command, CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                restaurantId => Created($"/api/restaurants/{restaurantId}", restaurantId),
                error => error.Problem());
        }
    }
}
