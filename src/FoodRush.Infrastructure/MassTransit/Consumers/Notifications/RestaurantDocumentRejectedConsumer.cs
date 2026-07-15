using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Features.Administration.Restaurants.IntegrationEvents;
using FoodRush.Infrastructure.Notifications.Models;
using MassTransit;

namespace FoodRush.Infrastructure.MassTransit.Consumers.Notifications;

internal sealed class RestaurantDocumentRejectedConsumer
    (IEmailService emailService,
    IEmailTemplateRenderer renderer)
    : IConsumer<RestaurantDocumentRejectedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<RestaurantDocumentRejectedIntegrationEvent> context)
    {
        var model = new RestaurantDocumentRejectedEmailModel(
            context.Message.OwnerName,
            context.Message.RestaurantName,
            context.Message.DocumentName,
            context.Message.Reason);

        var body = await renderer.RenderAsync(
            model,
            context.CancellationToken);

        await emailService.SendAsync(
            context.Message.OwnerEmail,
            "FoodRush - Document Review Required",
            body,
            context.CancellationToken);
    }
}
