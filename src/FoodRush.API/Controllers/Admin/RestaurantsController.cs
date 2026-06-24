using FoodRush.API.Extensions;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Restaurants.Queries.SearchRestaurants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
public class RestaurantsController(IMediator sender) : ControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> GetRestaurants(
        [FromQuery] SearchRestaurantsQuery query,
        CancellationToken cancellation)
    {
        var result = await sender.Send(query, cancellation);

        return result.Match(
            Ok,
            failure => failure.Problem()
        );
    }
}
