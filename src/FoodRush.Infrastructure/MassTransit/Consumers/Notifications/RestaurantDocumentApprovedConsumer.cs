using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Infrastructure.Notifications.Models;
using MassTransit;

namespace FoodRush.Infrastructure.MassTransit.Consumers.Notifications;

internal sealed class RestaurantDocumentApprovedConsumer
(
    IEmailTemplateRenderer2 renderer,
    IEmailService emailService
)
    : IConsumer<RestaurantDocumentApprovedIntegrationEvent>
{
    public async Task Consume(
        ConsumeContext<RestaurantDocumentApprovedIntegrationEvent> context)
    {
        RestaurantDocumentApprovedIntegrationEvent message =
            context.Message;

        string body =
            await renderer.RenderAsync(
                new RestaurantDocumentApprovedEmailModel
                {
                    OwnerName = message.OwnerName,
                    RestaurantName = message.RestaurantName
                },
                context.CancellationToken);

        await emailService.SendAsync(
            message.OwnerEmail,
            "Restaurant Documents Approved",
            body,
            context.CancellationToken);
    }
}