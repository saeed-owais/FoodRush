using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.BackgroundJobs;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Common;
using FoodRush.Domain.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FoodRush.Application.Features.Authentication.ResendVerificationEmail;

internal sealed class ResendVerificationEmailCommandHandler
    (
    IApplicationDbContext dbContext,
    IEmailVerificationTokenProvider tokenProvider,
    IBackgroundJobService backgroundJobService,
    IEmailTemplateRenderer emailTemplateRenderer,
    IOptions<FrontendSettings> options
    )
    : IRequestHandler<ResendVerificationEmailCommand, Result>
{
    private readonly FrontendSettings _frontendSettings = options.Value;

    public async Task<Result> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        string normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        User? user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result.Success(); // To prevent email enumeration, we return success even if the user doesn't exist  
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure(UserErrors.EmailAlreadyVerified);
        }

        var token = tokenProvider.GenerateToken(new EmailVerificationTokenPayload
            (user.Id,
             user.Email,
             user.SecurityStamp,
             DateTime.UtcNow.AddHours(1)));

        string verificationLink =
            $"{_frontendSettings.EmailVerificationUrl}?token={Uri.EscapeDataString(token)}";

        string emailBody = await emailTemplateRenderer.RenderAsync(
            new VerifyEmailModel(verificationLink),
            cancellationToken);

        backgroundJobService.Enqueue<IEmailService>(emailService => emailService.SendAsync(
            to: user.Email,
            subject: "Verify Your Email",
            body: emailBody,
            cancellationToken));

        return Result.Success();
    }
}
