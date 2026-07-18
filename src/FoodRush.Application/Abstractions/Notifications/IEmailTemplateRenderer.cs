namespace FoodRush.Application.Abstractions.Notifications;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync<TModel>(
        TModel model,
        CancellationToken cancellationToken = default)
        where TModel : class;
}