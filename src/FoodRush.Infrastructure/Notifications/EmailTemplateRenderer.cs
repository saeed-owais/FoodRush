using FoodRush.Application.Abstractions.Notifications;
using Scriban;
using System.Reflection;

namespace FoodRush.Infrastructure.Notifications;

internal sealed class EmailTemplateRenderer
    : IEmailTemplateRenderer
{
    public async Task<string> RenderAsync<TModel>(
       TModel model,
       CancellationToken cancellationToken = default)
       where TModel : class
    {
        var assembly = Assembly.GetExecutingAssembly();

        string resourceName =
            $"{assembly.GetName().Name}.Notifications.Templates.{typeof(TModel).Name}.sbn";

        var resources = assembly.GetManifestResourceNames();

        foreach (var resource in resources)
        {
            Console.WriteLine(resource);
        }
        await using Stream? stream =
            assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new FileNotFoundException(
                $"Email template '{resourceName}' was not found.");
        }

        using var reader = new StreamReader(stream);

        string templateText =
            await reader.ReadToEndAsync(cancellationToken);

        var template = Template.Parse(templateText);

        if (template.HasErrors)
        {
            throw new InvalidOperationException(
                string.Join(Environment.NewLine, template.Messages));
        }

        return await template.RenderAsync(model);
    }
}