namespace FoodRush.Application.Common.Settings;

public sealed class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";
    public string Host { get; init; } = default!;
    public ushort Port { get; init; }
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string VirtualHost { get; init; } = "/";
}
