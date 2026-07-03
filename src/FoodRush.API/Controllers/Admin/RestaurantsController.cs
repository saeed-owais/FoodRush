using FoodRush.API.Extensions;
using FoodRush.API.ViewModels;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Restaurants.Commands.ApproveRestaurantDocument;
using FoodRush.Application.Features.Administration.Restaurants.Commands.RejectRestaurantDocument;
using FoodRush.Application.Features.Administration.Restaurants.Queries.GetRestaurantDetailsForReview;
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

    [HttpGet("{restaurantId:guid}/review")]
    public async Task<IActionResult> GetRestaurantDetailsForReview(
        Guid restaurantId,
        CancellationToken cancellation)
    {
        var result = await sender.Send(
            new GetRestaurantDetailsForReviewQuery(restaurantId),
            cancellation);

        return result.Match(
            Ok,
            failure => failure.Problem());
    }

    [HttpPost("{restaurantId:guid}/documents/{documentId:guid}/approve")]
    public async Task<IActionResult> ApproveDocument(
        Guid restaurantId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var command = new ApproveRestaurantDocumentCommand(
            restaurantId,
            documentId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }

    [HttpPost("{restaurantId:guid}/documents/{documentId:guid}/reject")]
    public async Task<IActionResult> RejectDocument(
        Guid restaurantId,
        Guid documentId,
        RejectRestaurantDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RejectRestaurantDocumentCommand(
            restaurantId,
            documentId,
            request.Reason);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }

}
