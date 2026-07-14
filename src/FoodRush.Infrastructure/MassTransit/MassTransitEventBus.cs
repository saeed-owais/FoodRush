using FoodRush.Application.Abstractions.EventBus;
using MassTransit;

namespace FoodRush.Infrastructure.MassTransit;

internal class MassTransitEventBus(IPublishEndpoint publishEndpoint) : IEventBus
{

    public Task Publish<T>(T message, CancellationToken cancellationToken) where T : class
    {
        return publishEndpoint.Publish(message, cancellationToken);
    }
}
