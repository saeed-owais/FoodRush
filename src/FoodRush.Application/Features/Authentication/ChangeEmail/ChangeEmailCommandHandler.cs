using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.BackgroundJobs;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FoodRush.Application.Features.Authentication.ChangeEmail;

internal sealed class ChangeEmailCommandHandler
(
    IApplicationDbContext dbContext,
    IUserContext userContext,
    IBackgroundJobService backgroundJobService,
    IEmailChangeTokenProvider tokenProvider,
    IOptions<FrontendSettings> options
) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly FrontendSettings _frontendSettings = options.Value;
    public async Task<Result> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "User.NotFound",
                    $"User with ID {userContext.UserId} was not found."));
        }

        if (!user.IsEmailVerified)
        {
            return Result.Failure(
                Error.Conflict(
                    "Auth.EmailNotVerified",
                    "Current email must be verified."));
        }

        string normalizedEmail = request.NewEmail.Trim().ToUpperInvariant();

        bool emailAlreadyExists =
            await dbContext.Users.AnyAsync(
                u => u.NormalizedEmail == normalizedEmail
                     && u.Id != user.Id,
                cancellationToken);

        if (emailAlreadyExists)
        {
            return Result.Failure(
                Error.Conflict(
                    "Email.AlreadyExists",
                    "Email is already in use."));
        }


        bool isSameEmail = string.Equals(
            user.Email,
            request.NewEmail,
            StringComparison.OrdinalIgnoreCase);

        if (isSameEmail)
        {
            return Result.Failure(
                Error.Validation(
                    "Email.SameEmail",
                    "The new email must be different from the current email."));
        }

        string token =
            tokenProvider.GenerateToken(
                new EmailChangeTokenPayload(
                    user.Id,
                    request.NewEmail,
                    user.SecurityStamp,
                    DateTime.UtcNow.AddHours(24)));

        string confirmationLink = $"{_frontendSettings.ChangeEmailUrl}?token={Uri.EscapeDataString(token)}";

        backgroundJobService.Enqueue<IEmailService>(
            emailService => emailService.SendAsync(to: request.NewEmail,
                subject: "Confirm your email change",
                body: $"""
                      Please confirm your email change by clicking the following link:

                      {confirmationLink}

                      If you didn't request this change, you can safely ignore this email.
                      """
            ));

        return Result.Success();
    }
}
