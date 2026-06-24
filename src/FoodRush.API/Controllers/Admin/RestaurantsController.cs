using FoodRush.API.Extensions;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Restaurants.Queries.GetPendingRestaurants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
public class RestaurantsController(IMediator sender) : ControllerBase
{
    [HttpGet("drafts")]
    public async Task<IActionResult> GetDraftRestaurants(
        [FromQuery] GetDraftRestaurantsQuery query,
        CancellationToken cancellation)
    {
        var result = await sender.Send(query, cancellation);

        return result.Match(
            success => Ok(success),
            failure => failure.Problem()
        );
    }
}
