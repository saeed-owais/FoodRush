using FoodRush.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace FoodRush.Infrastructure.Notifications;

internal sealed class FakeEmailService(
    ILogger<FakeEmailService> logger)
    : IEmailService
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            """
            ========================================
            EMAIL SENT
            ========================================
            To: {To}
            Subject: {Subject}

            {Body}

            ========================================
            """,
            to,
            subject,
            body);

        return Task.CompletedTask;
    }
}