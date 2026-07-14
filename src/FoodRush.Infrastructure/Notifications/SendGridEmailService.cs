using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Common.Settings;
using FoodRush.Infrastructure.Resilience;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using SendGrid;

namespace FoodRush.Infrastructure.Notifications;

internal sealed class SendGridEmailService
(ISendGridClient sendGridClient,
    IOptions<SendGridSettings> options,
    ILogger<SendGridEmailService> logger,
    ResiliencePipelineProvider<string> pipelineProvider
) : IEmailService
{
    private readonly SendGridSettings _settings = options.Value;

    private readonly ResiliencePipeline _pipeline =
        pipelineProvider.GetPipeline(PipelineNames.SendEmail);

    [AutomaticRetry(Attempts = 0)]
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var from = new SendGrid.Helpers.Mail.EmailAddress(_settings.FromEmail, _settings.FromName);

        var toEmail = new SendGrid.Helpers.Mail.EmailAddress(to);

        var message = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);

        try
        {
            await _pipeline.ExecuteAsync(async token =>
            {
                Response response =
                    await sendGridClient.SendEmailAsync(
                        message,
                        token);

                if (!response.IsSuccessStatusCode)
                {
                    string error =
                        await response.Body.ReadAsStringAsync(token);

                    throw new EmailSendFailedException(
                        response.StatusCode,
                        $"SendGrid returned {(int)response.StatusCode}: {error}");
                }

            }, cancellationToken);

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