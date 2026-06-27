using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.BackgroundJobs;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Common;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FoodRush.Application.Features.Authentication.ForgotPassword;

internal sealed class ForgotPasswordCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordResetTokenProvider tokenProvider,
    IBackgroundJobService backgroundJobService,
    IEmailTemplateRenderer emailTemplateRenderer,
    IOptions<FrontendSettings> frontendSettings)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        string normalizedEmail = request.Email.Trim().ToUpperInvariant();

        User? user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result.Success();
        }

        if (!user.IsEmailVerified)
        {
            return Result.Success();
        }

        string token = tokenProvider.GenerateToken(new PasswordResetTokenPayload(
            user.Id,
            user.Email,
            user.SecurityStamp,
            DateTime.UtcNow.AddHours(1)));

        string resetLink = $"{frontendSettings.Value.ResetPasswordUrl}?token={Uri.EscapeDataString(token)}";

        string emailBody = emailTemplateRenderer.RenderResetPassword(resetLink);

        backgroundJobService.Enqueue<IEmailService>(emailService =>
            emailService.SendAsync(
                to: user.Email,
                subject: "Password Reset",
                body: emailBody));

        return Result.Success();
    }
}
