using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Infrastructure.Notifications.Models;
using MassTransit;

namespace FoodRush.Infrastructure.MassTransit.Consumers.Notifications;

internal sealed class RestaurantApprovedConsumer
    (IEmailService emailService,
    IEmailTemplateRenderer renderer)
    : IConsumer<RestaurantApprovedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RestaurantApprovedIntegrationEvent> context)
    {
        var body = await renderer.RenderAsync(new RestaurantApprovedEmailModel(
            context.Message.OwnerName,
            context.Message.RestaurantName));

        await emailService.SendAsync(
            context.Message.OwnerEmail,
            "Your Restaurant Has Been Approved",
            body,
            context.CancellationToken);
    }
}
