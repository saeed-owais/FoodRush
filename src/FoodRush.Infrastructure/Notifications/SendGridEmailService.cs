using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Common.Settings;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;

namespace FoodRush.Infrastructure.Notifications;

internal sealed class SendGridEmailService
(ISendGridClient sendGridClient,
    IOptions<SendGridSettings> options,
    ILogger<SendGridEmailService> logger
) : IEmailService
{
    private readonly SendGridSettings _settings = options.Value;

    [AutomaticRetry(Attempts = 0)]
    public async Task SendAsync(string to, string subject, string body)
    {
        var client = sendGridClient;

        var from = new SendGrid.Helpers.Mail.EmailAddress(_settings.FromEmail, _settings.FromName);

        var toEmail = new SendGrid.Helpers.Mail.EmailAddress(to);

        var message = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

        try
        {
            Response response = await client.SendEmailAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                string error =
                    await response.Body.ReadAsStringAsync();

                logger.LogError(
                    "SendGrid failed. StatusCode: {StatusCode}. Error: {Error}",
                    response.StatusCode,
                    error);

                throw new InvalidOperationException(
                    $"SendGrid failed: {error}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to send email to {Email}",
                to);

            throw;
        }
    }
}