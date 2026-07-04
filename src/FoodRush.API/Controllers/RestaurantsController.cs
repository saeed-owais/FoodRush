using FoodRush.API.Extensions;
using FoodRush.API.ViewModels;
using FoodRush.Application.Features.Restaurants.Onboarding;
using FoodRush.Application.Features.Restaurants.RegisterRestaurant;
using FoodRush.Application.Features.Restaurants.ResubmitDocument;
using FoodRush.Application.Features.Restaurants.SubmitForReview;
using FoodRush.Application.Features.Restaurants.UploadDocument;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestaurantsController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RegisterRestaurant(RegisterRestaurantCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            restaurantId => Created($"/api/restaurants/{restaurantId}", restaurantId),
            error => error.Problem());
    }

    [Authorize]
    [HttpPost("{restaurantId}/documents")]
    public async Task<IActionResult> UploadDocument(Guid restaurantId, [FromForm] UploadDocumentRequest request, CancellationToken cancellationToken)
    {
        using var stream = request.FileStream.OpenReadStream();

        var result = await sender.Send(
            new UploadDocumentCommand(
                restaurantId,
                request.DocumentType,
                stream,
                request.FileStream.FileName,
                request.FileStream.ContentType,
                stream.Length),
            cancellationToken);

        return result.Match(
            success => Created(
                $"api/restaurant/{success.RestaurantId}/documents/{success.Id.Value}",
                new { DocumentId = success.Id.Value }),
            error => error.Problem());
    }

    [Authorize]
    [HttpPost("{restaurantId}/documents/{documentId}/resubmit")]
    public async Task<IActionResult> ResubmitDocument(
    Guid restaurantId,
    Guid documentId,
    [FromForm] ResubmitDocumentRequest request,
    CancellationToken cancellationToken)
    {
        using var stream = request.FileStream.OpenReadStream();

        var command = new ResubmitDocumentCommand(
            restaurantId,
            documentId,
            stream,
            request.FileStream.FileName,
            request.FileStream.ContentType,
            request.FileStream.Length);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            Ok,
            error => error.Problem());
    }

    [Authorize]
    [HttpPost("{restaurantId:Guid}/review")]
    public async Task<IActionResult> SubmitForReview(Guid restaurantId, CancellationToken cancellationToken)
    {
        var command = new SubmitForReviewCommand(restaurantId);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            Ok,
            error => error.Problem());
    }

    [Authorize]
    [HttpGet("{restaurantId:Guid}/onboarding")]
    public async Task<IActionResult> GetMyDocuments(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRestaurantOnboardingQuery(restaurantId), cancellationToken);

        return result.Match(
            success => Ok(success),
            error => error.Problem());
    }
}


