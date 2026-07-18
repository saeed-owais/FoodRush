namespace FoodRush.Application.Abstractions.EventBus;

public interface IEventBus
{
    Task Publish<T>(T message, CancellationToken cancellationToken) where T : class;
}
